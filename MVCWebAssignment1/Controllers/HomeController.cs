﻿using Microsoft.AspNet.Identity;
using MVCWebAssignment1.DAL;
using MVCWebAssignment1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using MVCWebAssignment1.Common_Logic;

namespace MVCWebAssignment1.Controllers
{
    public class HomeController : Controller
    {

        private readonly IEventRepository _eventRepository;
        private ILaneRepository _laneRepository;
        private Library _library;

        public HomeController()
        {
            _eventRepository = new EventRepository(new EventContext());
            _laneRepository = new LaneRepository(new LaneContext());
            _library = new Library();
        }


        public ViewResult Index(string searchParamVenue, string searchParamAge, string searchParamDistance, string searchParamStroke, string searchParamGender)
        {
            IList<Event> events = _eventRepository.GetEvents();

            if(!String.IsNullOrEmpty(searchParamVenue))
            {
                events = events.Where(x => x.Meet.Venue.VenueName.Contains(searchParamVenue)).ToList();
            } 
            if(!String.IsNullOrEmpty(searchParamAge))
            {
                events = events.Where(x => x.AgeRange.Contains(searchParamAge)).ToList();
            }
            if(!String.IsNullOrEmpty(searchParamDistance))
            {
                events = events.Where(x => x.Distance.Contains(searchParamDistance)).ToList();
            }
            if(!String.IsNullOrEmpty(searchParamStroke))
            {
                events = events.Where(x => String.Equals(x.SwimmingStroke, searchParamStroke, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            if(!String.IsNullOrEmpty(searchParamGender))
            {
                events = events.Where(x => String.Equals(x.Gender, searchParamGender, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }

            HomeViewModel homeViewModel = new HomeViewModel();
            homeViewModel.Events = events;

            if (User.IsInRole("Parent"))
            {
                var familyGroupId = _library.GetFamilyId(User.Identity.GetUserId());

                if (familyGroupId != 0)
                {
                    homeViewModel.FamilyGroupId = familyGroupId;
                }
            }

            string swimmerId = User.Identity.GetUserId();

            ViewBag.SwimmerId = swimmerId;

            List<Event> personalEvents = new List<Event>();

            foreach (var @event in _eventRepository.GetEvents())
            {
                foreach (var round in @event.Rounds)
                {
                    foreach (var lane in round.Lanes)
                    {
                        if (lane.SwimmerId == swimmerId)
                        {
                            personalEvents.Add(@event);
                        }
                    }
                }

            }

            if(personalEvents != null)
            {
                homeViewModel.PersonalEvents = personalEvents;
            }
 
            return View(homeViewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}