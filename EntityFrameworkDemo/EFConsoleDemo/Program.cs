using EFDemo.Data;
using EFDemo.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFConsoleDemo
{
    class Program
    {
        private static PortalContext context = new PortalContext();

        static async Task Main(string[] args)
        {

            //await AddNewLeague();
            //await AddNewTeamsWithLeague();

            //await SimpleSelectQuery()
            
            await QueryFilters();

            Console.WriteLine("press any key");
            Console.ReadKey();
        }


        static async Task QueryFilters()
        {
            //Console.WriteLine($"Enter League Name: ");
            Console.WriteLine($"Enter League Name (Or Part Of): ");
            var leagueName = Console.ReadLine();
            var exactMatches = await context.Leagues.Where(q => q.Name.Equals(leagueName)).ToListAsync();
            foreach (var league in exactMatches)
            {
                Console.WriteLine($"{league.Id} . {league.Name}");
            }

            //var partialMatches = await context.Leagues.Where(q => q.Name.Contains(leagueName)).ToListAsync();
            var partialMatches = await context.Leagues.Where(q => EF.Functions.Like(q.Name,$"%{leagueName}%")).ToListAsync();
            foreach (var league in partialMatches)
            {
                Console.WriteLine($"{league.Id} . {league.Name}");
            }
        }
        
        static async Task SimpleSelectQuery()
        {
            var leagues = await context.Leagues.ToListAsync(); // ToList executes the query, converts to list of objects, then the database conection closes
            foreach (var league in leagues)
            {
                Console.WriteLine($"{league.Id} . {league.Name}");
            }
        }
        
        static async Task AddNewLeague()
        {
            //add new league object
            var league = new League { Name = "Serie A" };
            await context.Leagues.AddAsync(league);
            await context.SaveChangesAsync();

            //add new teams related to the new league object
            await AddTeamsWithLeague(league);
            await context.SaveChangesAsync();
        }
        
        static async Task AddTeamsWithLeague(League league)
        {
            var teams = new List<Team>
            {
                new Team
                {
                    Name = "Juventus",
                    LeagueId = league.Id
                },
                new Team
                {
                    Name = "AC Milan",
                    LeagueId = league.Id
                },
                new Team
                {
                    Name = "AS Roma",
                    League = league
                }
            };

            //add multiple objects to database at once
            await context.AddRangeAsync(teams);
        }
        
        static async Task AddNewTeamsWithLeague()
        {
            var league = new League { Name = "Bundesliga" };
            var team = new Team { Name = "Bayern Munich", League = league };
            await context.AddAsync(team);
            await context.SaveChangesAsync();
        }
    }
}
