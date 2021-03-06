﻿using ExampleAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAPI.Services
{
    public interface IEventService
    {
        EventModel CreateEvent(EventModel eventModel);
        EventModel GetEvent(int Id);
        List<EventModel> GetEvents();
        List<EventModel> GetEventsByUser(int Id);

        

    }

    // normal CRUD
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;
        public EventService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public List<EventModel> GetEventsByUser(int id)
        {
            return _context.Events.Where(e => e.CreatorId == id).ToList();
        }


        public EventModel CreateEvent(EventModel eventModel)
        {
            _context.Events.Add(eventModel);
            _context.SaveChanges();
            return eventModel;
            
        }

        public EventModel GetEvent(int Id)
        {

            var response = _context.Events
                .SingleOrDefault(e => e.Id == Id);

            return response;

        }

        public List<EventModel> GetEvents()
        {
            return _context.Events
                .ToList();
        }


    }
}
