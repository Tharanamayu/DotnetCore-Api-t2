using AutoMapper;
using CoreCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Data
{
    public class CampProfile:Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampModel>()//map camp entity to campModel
                .ForMember(c => c.Venue, o => o.MapFrom(m => m.Location.VenueName))//add location data(Vanue) to MapModel from the Location entity
                .ReverseMap();
            this.CreateMap<Speaker, SpeakerModel>()
                .ReverseMap(); ;
            this.CreateMap<Talk, TalkModel>()
                .ReverseMap()
                .ForMember(t=>t.Camp,opt=>opt.Ignore())
                .ForMember(t => t.Speaker, opt => opt.Ignore());
        }
    }
}
