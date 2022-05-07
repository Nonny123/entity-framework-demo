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

            //await QueryFilters();

            //await AdditionalExecutionMethods();

            //await AlternativeLinqSyntax();

            //await UpdateLeagueRecord();

            //await UpdateTeamRecord();

            //await Delete();

            //await DeleteWithRelationship();

            Console.WriteLine("press any key");
            Console.ReadKey();
        }

        private static async Task TrackingVsNoTracking()
        {
            var withTracking = await context.Teams.FirstOrDefaultAsync(q => q.Id == 2);

            //releases memory and speeds up performance because objects are not tracked
            var withNoTracking = await context.Teams.AsNoTracking().FirstOrDefaultAsync(q => q.Id == 4);

            withTracking.Name = "";
            withNoTracking.Name = "";

            var entriesBeforeSave = context.ChangeTracker.Entries();

            await context.SaveChangesAsync();

            var entriesAfterSave = context.ChangeTracker.Entries();

        }

        private static async Task Delete()
        {
            var league = await context.Leagues.FindAsync(4);
            context.Leagues.Remove(league);
            await context.SaveChangesAsync();
        }

        private static async Task DeleteWithRelationship()
        {
            var league = await context.Leagues.FindAsync(5);
            context.Leagues.Remove(league);
            await context.SaveChangesAsync();
        }



        private static async Task UpdateTeamRecord()
        {
            var team = new Team
            {
                Id = 4,
                Name = "Arsenal",
                LeagueId = 1
            };
            context.Teams.Update(team);
            await context.SaveChangesAsync();
        }

        private static async Task GetRecord()
        {
            var league = await context.Leagues.FindAsync(2);
            Console.WriteLine($"{league.Id} . {league.Name}");
        }

        private static async Task UpdateLeagueRecord()
        {
            var league = await context.Leagues.FindAsync(2);

            league.Name = "Nigerian Premiership";

            await context.SaveChangesAsync();

            await GetRecord();
        }


        static async Task AlternativeLinqSyntax()
        {
            Console.WriteLine($"Enter Team Name (Or Part Of): ");
            var teamName = Console.ReadLine();

            var teams = await (from i in context.Teams
                                   //where i.Name == ""
                               where EF.Functions.Like(i.Name, $"%{teamName}%")
                               select i).ToListAsync();

            foreach(var team in teams)
            {
                Console.WriteLine($"{team.Id} . {team.Name}");
            }
        }


        static async Task AdditionalExecutionMethods()
        {
            //var l = context.Leagues.Where(q => q.Name.Contains("A")).FirstOrDefaultAsync();
            var leagues = context.Leagues;
            var list = await leagues.ToListAsync();
            var first = await leagues.FindAsync();
            var firstOrDefault = await leagues.FirstOrDefaultAsync();
            var single = await leagues.SingleAsync();
            var singleOrDefault = await leagues.SingleOrDefaultAsync();

            var count = await leagues.CountAsync();
            var longCount = await leagues.LongCountAsync();
            var min = await leagues.MinAsync();
            var max = await leagues.MaxAsync();

            var league = await leagues.FindAsync(1);


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
