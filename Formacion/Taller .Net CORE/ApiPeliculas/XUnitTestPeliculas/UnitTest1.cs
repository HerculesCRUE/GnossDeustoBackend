using ApiPeliculas.Controllers;
using ApiPeliculas.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Xunit;
using XUnitTestPeliculas.Mock;

namespace XUnitTestPeliculas
{
    public class UnitTest1
    {
        [Fact]
        public void ListFilmsOk()
        {
            DataFilmMockService mock = new DataFilmMockService();
            FilmController controllerMock = new FilmController(mock);
            List<Film> films = (List<Film>)((OkObjectResult)controllerMock.GetFilms()).Value;
            if (films.Count > 0)
            {
                Assert.True(true);
            }
            else
            {
                Assert.True(false);
            }
        }
    }
}
