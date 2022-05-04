using EFDemo.Data;
using EFDemo.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EFConsoleDemo
{
    class Program
    {
        private static PortalContext context = new PortalContext();

        static async Task Main(string[] args)
        {
            //var league = new League { Name = "Bundesliga" };
            //var team = new Team { Name = "Bayern Munich", League = league };
            //await context.AddAsync(team);
            //await context.SaveChangesAsync();


            var league = new League { Name = "Serie A" };
            await context.Leagues.AddAsync(league);
            await context.SaveChangesAsync();

            
            await AddTeamsWithLeague(league);
            await context.SaveChangesAsync();

            Console.WriteLine("press any key");
            Console.ReadKey();
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

            await context.AddRangeAsync(teams);
        }
    }
}
