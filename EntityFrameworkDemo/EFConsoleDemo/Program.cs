using EFDemo.Data;
using EFDemo.Domain;
using EFDemo.Domain.Models;
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

            //await AddNewTeamsWithLeagueId();

            //await AddNewLeagueWithTeams();

            //await AddNewMatches();

            //await AddNewCoach();

            await QueryRelatedRecords();

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
  
        static async Task SimpleSelectQuery()
        {
            var leagues = await context.Leagues.ToListAsync(); // ToList executes the query, converts to list of objects, then the database conection closes
            foreach (var league in leagues)
            {
                Console.WriteLine($"{league.Id} . {league.Name}");
            }
        }


        #region Adding

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

        //the user selects dropdown for where the team will belong to
        static async Task AddNewTeamsWithLeagueId()
        {
            var team = new Team { Name = "Inter Milan", LeagueId = 3 };
            await context.AddAsync(team);
            await context.SaveChangesAsync();
        }

        //create new league and add some teams in the league
        static async Task AddNewLeagueWithTeams()
        {
            var teams = new List<Team> {
                new Team
                {
                    Name = "PSG"
                },
                new Team
                {
                    Name = "Lyon"
                },
                new Team
                {
                    Name = "Lille"
                }
            };
            var league = new League { Name = "French Ligue One", Teams = teams };
            await context.AddAsync(league);
            await context.SaveChangesAsync();
        }

        static async Task AddNewMatches()
        {
            //Bundesliga matches
            var matches = new List<Match>
            {
                new Match
                {
                    AwayTeamId = 7, HomeTeamId = 8, Date = new DateTime(2022, 08, 08)
                },
                new Match
                {
                    AwayTeamId = 9, HomeTeamId = 13, Date = new DateTime(2022, 08, 09)
                },
                new Match
                {
                    AwayTeamId = 14, HomeTeamId = 15, Date = new DateTime(2022, 08, 10)
                }
            };
            await context.AddRangeAsync(matches); //AddRange is for adding list of items
            await context.SaveChangesAsync();
        }

        static async Task AddNewCoach()
        {
            var coach1 = new Coach { Name = "Mikel Arterta", TeamId = 4 };
            await context.AddAsync(coach1);

            var coach2 = new Coach { Name = "Frank Lampard"}; //nullable int -> no team to coach at the moment
            await context.AddAsync(coach2);

            await context.SaveChangesAsync();
        }

        #endregion


        #region GetRelatedRecords

        //eager loading - Include method
        static async Task QueryRelatedRecords()
        {
            //get many related records
            //left join - matching value is (league id/team id) - return all rows from teams and only rows from league tables
            // matching the matching value
            var league = await context.Leagues.Include(q => q.Teams).ToListAsync();

            //get one related record
            //left join - matching value is (team id) - return all rows from coach and only rows from team table
            //matching the matching value
            var team = await context.Teams
                .Include(q => q.Coach)
                .FirstOrDefaultAsync(q => q.Id == 4); //get arsenal with its coach

            //get grandchildern - Team -> Matches -> Home/Away Team
            var teamsWithMatchesAndOpponentss = await context.Teams
                .Include(q => q.AwayMatches).ThenInclude(q => q.HomeTeam).ThenInclude(q => q.Coach)
                .Include(q => q.HomeMatches).ThenInclude(q => q.AwayTeam).ThenInclude(q => q.Coach)
                .FirstOrDefaultAsync(q => q.Id == 1);

            //get include with filters
            var teams = await context.Teams
                .Where(q => q.HomeMatches.Count > 0)
                .Include(q => q.Coach)
                .ToListAsync();
        }


        #endregion

        #region ProjectionsToOtherTypes/AnonymousTypes

        static async Task SelectOneProperty()
        {
            var teams = await context.Teams.Select(q => q.Name).ToListAsync();
        }

        static async Task AnonymousProjection()
        {
            var teams = await context.Teams.Include(q => q.Coach).Select( //project to a type/object/anonymous object so you can select more columns
                q => 
                new { 
                        TeamName = q.Name,
                        CoachName = q.Coach.Name
                    }
                ).ToListAsync();

            foreach (var item in teams)
            {
                Console.WriteLine($"Team: {item.TeamName} | Coach: {item.CoachName}");
            }
        }

        static async Task StrongTypedProjection()
        {
            var teams = await context.Teams.Include(q => q.Coach).Include(q => q.League).Select( //project to a type/object/anonymous object so you can select more columns
                q =>
                new TeamDetail{
                    Name = q.Name,
                    CoachName = q.Coach.Name,
                    LeagueName = q.League.Name
                }
                ).ToListAsync();

            foreach (var item in teams)
            {
                Console.WriteLine($"Team: {item.Name} | Coach: {item.CoachName} | League: {item.LeagueName}");
            }
        }

        #endregion


        #region Filtering

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
            var partialMatches = await context.Leagues.Where(q => EF.Functions.Like(q.Name, $"%{leagueName}%")).ToListAsync();
            foreach (var league in partialMatches)
            {
                Console.WriteLine($"{league.Id} . {league.Name}");
            }
        }


        //query/show leagues and allow to filter based on league name
        //Basically use a team's name to get the league/list of leagues which the team belongs to
        static async Task FilteringWithRelatedData()
        {
           var league = await context.Leagues.Where(q => q.Teams.Any(x => x.Name.Contains("Arsen"))).ToListAsync();
        }

        #endregion
    }
}
