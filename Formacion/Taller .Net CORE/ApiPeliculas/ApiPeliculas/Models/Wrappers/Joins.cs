using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiPeliculas.Models.Entities;

namespace ApiPeliculas.Models.Wrappers
{
    public static class Joins
    {
        public static IQueryable<ActorPerson> JoinPerson(this IQueryable<Actor> pQuery, EntityContext context)
        {
            return pQuery.Join(context.Person, actor => actor.Person_ID, person => person.Person_ID, (actor, person) => new ActorPerson
            {
                Actor = actor,
                Person = person
            });
        }
        public static IQueryable<ActorPersonFilmActor> JoinFilmActor(this IQueryable<ActorPerson> pQuery, EntityContext context)
        {
            return pQuery.Join(context.FilmActor, item => item.Actor.Actor_ID, fa => fa.Actor_ID, (item, fa) => new ActorPersonFilmActor
            {
                Actor = item.Actor,
                Person = item.Person,
                FilmActor = fa
            });
        }

        public static IQueryable<ActorPersonFilmActorFilm> JoinFilm(this IQueryable<ActorPersonFilmActor> pQuery, EntityContext context)
        {
            return pQuery.Join(context.Film, item => item.FilmActor.Film_ID, film => film.Film_ID, (item, film) => new ActorPersonFilmActorFilm
            {
                Actor = item.Actor,
                Person = item.Person,
                FilmActor = item.FilmActor,
                Film = film
            });
        }
    }

    public class ActorPerson
    {
        public Actor Actor { get; set; }
        public Person Person { get; set; }
    }

    public class ActorPersonFilmActor
    {
        public Actor Actor { get; set; }
        public Person Person { get; set; }
        public FilmActor FilmActor { get; set; }
    }
    public class ActorPersonFilmActorFilm
    {
        public Actor Actor { get; set; }
        public Person Person { get; set; }
        public FilmActor FilmActor { get; set; }
        public Film Film { get; set; }
    }
}
