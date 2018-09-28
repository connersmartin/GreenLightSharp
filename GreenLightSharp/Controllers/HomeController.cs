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
            //TODO Figure this out for adding a member
            //Will need to know bandId for proper linkage can this be saved with a token?
            //may need to implement a secondary table if these are 'universal/session' members, but that's definitely secondary

            Member bandMember = new Member { Name = model.Name, Instrument = model.Instrument, Status = "0", BandId = model.BandId };

            Show band = new Show { Members = new List<Member> { bandMember } };

            // /member?&name=***
            //API call to add a new member
            //Need to figure out way to link bands and members 'easily'
            //DOESNT work yet

            GoogleRest(bandMember, Method.POST, bandMember.BandId + "/Member");

            //We actually need to send the serialized band object to the view I think

            band = JsonConvert.DeserializeObject<Show>(GoogleRest(null, Method.GET, bandMember.BandId));

            Dictionary<string, Member> members =  JsonConvert.DeserializeObject<Dictionary<string,Member>>(GoogleRest(null, Method.GET, model.BandId + "/Member"));

            foreach (Member m in members.Values)
            {
                band.Members.Add(m);
            }

            //this restResponse.content may be the best way to link bands/members
            //Goes to show page
            return View("ShowPage", band);
        }

        public ActionResult ShowPage(Show band)
        {
            //Where everything is displayed and updated
 
            return View(band);
        }

        public string GoogleRest(object obj, Method method, string resource = null)
        {
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