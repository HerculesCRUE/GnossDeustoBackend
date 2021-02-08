using ApiPeliculas.Models;
using ApiPeliculas.Models.Entities;
using ApiPeliculas.Models.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ApiPeliculas
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "api peliculas", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
            services.AddEntityFrameworkSqlServer().AddDbContext<EntityContext>(options =>
              options.UseSqlServer(Configuration.GetConnectionString("FilmContext"))
              
              );

            services.AddScoped<IDirectorDataService, DirectorDataService>();
            services.AddScoped<IActorDataService, ActorDataService>();
            services.AddScoped<IFilmDataService, FilmDataService>();

            var sp = services.BuildServiceProvider();

            // Resolve the services from the service provider
            var context = sp.GetService<EntityContext>();
            InitializeDatabase(context);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Servers = new List<OpenApiServer>
                      {
                        new OpenApiServer { Url = $"/" }
                      });
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "api peliculas");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void InitializeDatabase(EntityContext context)
        {

            if (context.FilmActor.Any())
            {
                foreach (var filmActor in context.FilmActor)
                {
                    context.FilmActor.Remove(filmActor);
                }
                context.SaveChanges();
            }
            if (context.Film.Any())
            {
                foreach (var film in context.Film)
                {
                    context.Film.Remove(film);
                }
                context.SaveChanges();
            }
            if (context.Director.Any())
            {
                foreach (var director in context.Director)
                {
                    context.Director.Remove(director);
                }
                context.SaveChanges();
            }
            if (context.Rating.Any())
            {
                foreach (var rating in context.Rating)
                {
                    context.Rating.Remove(rating);
                }
                context.SaveChanges();
            }


            if (context.Actor.Any())
            {
                foreach (var actor in context.Actor)
                {
                    context.Actor.Remove(actor);
                }
                context.SaveChanges();
            }
            if (context.Person.Any())
            {
                foreach (var person in context.Person)
                {
                    context.Person.Remove(person);
                }
                context.SaveChanges();
            }
            Guid personID1 = Guid.NewGuid();
            Person person1 = new Person()
            {
                Name = "George Lucas",
                Person_ID = personID1
            };

            Guid personID2 = Guid.NewGuid();
            Person person2 = new Person()
            {
                Name = "Ewan McGregor",
                Person_ID = personID2
            };

            Guid personID3 = Guid.NewGuid();
            Person person3 = new Person()
            {
                Name = "Hayden Christensen",
                Person_ID = personID3
            };

            Guid personID4 = Guid.NewGuid();
            Person person4 = new Person()
            {
                Name = "Brad Pitt",
                Person_ID = personID4
            };

            Guid personID5 = Guid.NewGuid();
            Person person5 = new Person()
            {
                Name = "Quentin Tarantino",
                Person_ID = personID5
            };

            context.Person.Add(person1);
            context.Person.Add(person2);
            context.Person.Add(person3);
            context.Person.Add(person4);
            context.Person.Add(person5);
            context.SaveChanges();

            Guid actorID1 = Guid.NewGuid();
            Actor actor1 = new Actor()
            {
                Actor_ID = actorID1,
                Person_ID = personID2
            };
            Guid actorID2 = Guid.NewGuid();
            Actor actor2 = new Actor()
            {
                Actor_ID = actorID2,
                Person_ID = personID3
            };
            Guid actorID3 = Guid.NewGuid();
            Actor actor3 = new Actor()
            {
                Actor_ID = actorID3,
                Person_ID = personID4
            };
            context.Actor.Add(actor1);
            context.Actor.Add(actor2);
            context.Actor.Add(actor3);
            Guid directorID1 = Guid.NewGuid();
            Director director1 = new Director()
            {
                Director_ID = directorID1,
                Person_ID = personID1
            };

            Guid directorID2 = Guid.NewGuid();
            Director director2 = new Director()
            {
                Director_ID = directorID2,
                Person_ID = personID5
            };
            context.Director.Add(director1);
            context.Director.Add(director2);
            context.SaveChanges();

            Guid filmIDSW = Guid.NewGuid();
            Film film1 = new Film()
            {
                MinuteRunTime = 139,
                Director_ID = directorID1,
                Title = "Star Wars: Episodio III - La venganza de los Sith",
                Year = 2005,
                Film_ID = filmIDSW
            };

            Guid filmIDSW2 = Guid.NewGuid();
            Film film2 = new Film()
            {
                MinuteRunTime = 142,
                Director_ID = directorID1,
                Title = "Star Wars: Episodio II - El ataque de los clones",
                Year = 2002,
                Film_ID = filmIDSW2
            };

            Guid filmIDMB = Guid.NewGuid();
            Film filmMB = new Film()
            {
                MinuteRunTime = 153,
                Director_ID = directorID2,
                Title = "Malditos bastardos",
                Year = 2009,
                Film_ID = filmIDMB
            };
            context.Film.Add(film1);
            context.Film.Add(film2);
            context.Film.Add(filmMB);
            context.SaveChanges();
            FilmActor fmSW1 = new FilmActor()
            {
                Actor_ID = actorID1,
                Film_ID = filmIDSW
            };

            FilmActor fmSW2 = new FilmActor()
            {
                Actor_ID = actorID2,
                Film_ID = filmIDSW
            };
            FilmActor fmSW21 = new FilmActor()
            {
                Actor_ID = actorID2,
                Film_ID = filmIDSW2
            };
            FilmActor fmSW22 = new FilmActor()
            {
                Actor_ID = actorID1,
                Film_ID = filmIDSW2
            };

            FilmActor filmActorMB = new FilmActor()
            {
                Actor_ID = actorID3,
                Film_ID = filmIDMB
            };

            context.FilmActor.Add(fmSW1);
            context.FilmActor.Add(fmSW2);
            context.FilmActor.Add(fmSW21);
            context.FilmActor.Add(fmSW22);
            context.FilmActor.Add(filmActorMB);
            context.SaveChanges();
        }
    }
}
