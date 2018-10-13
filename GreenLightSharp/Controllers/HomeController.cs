using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GreenLightSharp.Models;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;

namespace GreenLightSharp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string error = null)
        {
            if (error == "doesnotexist")
            {
                ViewBag.Message = "Band Id does not exist, please try again";
            }
            else if (error == "pleaseenterbandname")
            {
                ViewBag.Message = "Please enter a band name and a number > 1";
            }

            return View();
        }

        public ActionResult Help()
        {
            ViewBag.Message = "How to use this app.";
            return View();
        }

        [HttpPost]
        public ActionResult AddBand(Show model)
        {
            model.ShowStatus = "0";
            if (ModelState.IsValid)
            {
                //Create band with a given name
                //bandid is returned from
                Member mem = new Member { BandId = JsonConvert.DeserializeObject<Dictionary<string, string>>(GoogleRest(model, Method.POST)).Values.FirstOrDefault() };

                //updates the band with its firebase id
                model.Id = mem.BandId;
                GoogleRest(model, Method.PATCH, mem.BandId );

                //Go to AddMember view with bandId showing
                return RedirectToAction("AddMember", mem);
            }
            return RedirectToAction("index", new { error = "pleaseenterbandname" });
        }

        [HttpPost]
        public ActionResult Join(Member model)
        {
            //basic validation
            try
            {
                GetAndReturnBand(model.BandId);

                return RedirectToAction("AddMember", model);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { error = "doesnotexist" });
            }   
            //Goes to add a member page with given bandId      
        }

        public ActionResult AddMember(Member model)
        {
            //Add Member page
            //Need to figure out how to do this with partial views keeping the same front page essentially
            return View(model);
        }

        [HttpPost]
        public ActionResult PostMember(Member model)
        {
            //Add Member
            //Will need to know bandId for proper linkage can this be saved with a token?
            //may need to implement a secondary table if these are 'universal/session' members, but that's definitely secondary

            Member bandMember = new Member {
                Id =null,
                Name = model.Name,
                Instrument = model.Instrument,
                Status = "0",
                BandId = model.BandId
            };

            //API call to add a new member and give them an id

            bandMember.Id = JsonConvert.DeserializeObject<Dictionary<string, string>>(CreateMember(bandMember)).FirstOrDefault().Value;
            //update to give bandmember own id
            UpdateMember(bandMember);

            //Goes to show page
            return RedirectToAction("ShowPage", new { id = bandMember.Id, bid = bandMember.BandId });
        }

        [HttpGet]
        public ActionResult ShowPage(string id, string bid)
        {
            //Where everything is displayed and updated

            //get the band
            Display band = GetAndReturnBand(bid);            
            band.Member = JsonConvert.DeserializeObject<Member>(GoogleRest(null, Method.GET, bid + "/Member/" + id));
            //Want to make sure there are at least the number of members you wanted ready before success
            if (IsReady(band.Show) && band.Show.Size >= band.Show.Members.Count)
            {
                band.Show.ShowStatus = "1";
                UpdateShowStatus(band.Show);
            }

            return View(band);
        }

        [HttpPost]
        public ActionResult PatchMember(string id, string bid)
        {
            Display band = GetAndReturnBand(bid);
            band.Member = JsonConvert.DeserializeObject<Member>(GoogleRest(null, Method.GET, bid + "/Member/" + id));

            if (band.Member.Status == "0")
            {
                band.Member.Status = "1";
            }
            else
            {
                band.Member.Status = "0";
            }
            band.Member = JsonConvert.DeserializeObject<Member>(UpdateMember(band.Member));

            return RedirectToAction("ShowPage", new { id = band.Member.Id, bid = band.Member.BandId });
        }

        public bool IsReady(Show band)
        {
            //Check to see if each member is ready
            foreach (Member mem in band.Members)
            {
                if (mem.Status == "0" || band.Members.Count == 1)
                {
                    return false;
                }
            }
            return true;
        }

        public void Clear(string bid)
        {
            Display show = GetAndReturnBand(bid);
            //Clear status for each member
            foreach (Member mem in show.Show.Members)
            {
                mem.Status = "0";
                UpdateMember(mem);
            }
            //Clear show status
            show.Show.ShowStatus = "0";
            UpdateShowStatus(show.Show);
        }

        public Display GetAndReturnBand(string bandId)
        {
            Display showtime = new Display {
                //Gets the band
                Show = JsonConvert.DeserializeObject<Show>(GoogleRest(null, Method.GET, bandId))
            };

            //Gets the Members
            Dictionary<string, Member> members = JsonConvert.DeserializeObject<Dictionary<string, Member>>(GoogleRest(null, Method.GET, bandId + "/Member"));
                //adds them if there are any
                foreach (Member m in members.Values)
                {
                    showtime.Show.Members.Add(m);
                }
            return showtime;
        }

        public string UpdateShowStatus(Show band)
        {
            return GoogleRest(new Dictionary<string, string> { { "ShowStatus", band.ShowStatus } }, Method.PATCH, band.Id+"/");
        }

        public string UpdateMember(Member member)
        {
            return GoogleRest(member, Method.PATCH, member.BandId + "/Member/" + member.Id);
        }

        public string CreateMember(Member member)
        {
            return GoogleRest(member, Method.POST, member.BandId + "/Member");
        }

        public string GoogleRest(object obj, Method method, string resource = null)
        {
            //Generic API call for firebase db
            var client = new RestClient();
            client.BaseUrl = new Uri("");
            var json = JsonConvert.SerializeObject(obj);
            var request = new RestRequest(method);
            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            request.Resource = string.Format("{0}.json", resource);
            IRestResponse restResponse = client.Execute(request);

            return restResponse.Content;
        }
    }
}