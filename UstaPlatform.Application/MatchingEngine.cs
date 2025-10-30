using System;
using System.Collections.Generic;
using System.Linq;
using UstaPlatform.Domain;
using UstaPlatform.Infrastructure; 

namespace UstaPlatform.Application
{
    public class MatchingEngine
    {
        public Master? FindBestMatch(Request request, IEnumerable<Master> masters)
        {
            if (masters == null || !masters.Any())
                return null;

            var filteredMasters = masters.Where(m => m.Expertise == request.RequiredExpertise).ToList();

            if (!filteredMasters.Any())
                return null;

            int minLoad = filteredMasters.Min(m => m.CurrentLoad);
            var candidateMasters = filteredMasters.Where(m => m.CurrentLoad == minLoad).ToList();

            if (!candidateMasters.Any())
            {
                candidateMasters = filteredMasters;
            }

            var bestMatchResults = candidateMasters
                .Select(m => new
                {
                    Master = m,
                    DistanceKm = DistanceHelper.CalculateDistance(request.Requester.Address, m.Address)
                })
                .OrderBy(r => r.DistanceKm) 
                .ToList();
                Random rand = new Random();
            var finalCandidate = bestMatchResults
                .Take(3)
                .OrderBy(r => rand.Next()) 
                .FirstOrDefault();

            return finalCandidate?.Master;
        }
    }
}