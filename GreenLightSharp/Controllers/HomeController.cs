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
        public ActionResult Index()
        {
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
            //Create band with a given name
            //bandid is returned from
            Member mem = new Member { BandId = JsonConvert.DeserializeObject<Dictionary<string, string>>(GoogleRest(model, Method.POST)).Values.FirstOrDefault() };

            //updates the band with its firebase id
            model.Id = mem.BandId;
            GoogleRest(model, Method.PATCH, mem.BandId );

            //Go to AddMember view with bandId showing
            return RedirectToAction("AddMember", mem);
        }

        [HttpPost]
        public ActionResult Join(Member model)
        {
            //Goes to add a member page with given bandId
            return RedirectToAction("AddMember", model);
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

            Member bandMember = new Member { Name = model.Name, Instrument = model.Instrument, Status = "0", BandId = model.BandId };

            
            // /member?&name=***
            //API call to add a new member

            GoogleRest(bandMember, Method.POST, bandMember.BandId + "/Member");

            //get the band
            Display display = GetAndReturnBand(model.BandId);

            //Add your person
            display.Member = bandMember;

            //Goes to show page
            return View("ShowPage", display);
        }

        public ActionResult ShowPage(Display band)
        {
            //Where everything is displayed and updated
 
            return View(band);
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

        public Display GetAndReturnBand(string bandId)
        {
            Display showtime = new Display {
                //Gets the band
                Show = JsonConvert.DeserializeObject<Show>(GoogleRest(null, Method.GET, bandId))
            };

            //Gets the Members
            Dictionary<string, Member> members = JsonConvert.DeserializeObject<Dictionary<string, Member>>(GoogleRest(null, Method.GET, bandId + "/Member"));

            //adds them
            foreach (Member m in members.Values)
            {
                showtime.Show.Members.Add(m);
            }

            return showtime;
        }
    }
}