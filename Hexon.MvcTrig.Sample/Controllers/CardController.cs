using Hexon.MvcTrig.Sample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hexon.MvcTrig.Sample.Controllers
{
    public class CardController : Controller
    {
        private static IList<Card> Cards { get; } = new List<Card>
        {
            new Card { Id = 1, Name = "Bibby Be", Phone = "0900000001" },
            new Card { Id = 2, Name = "Bruce Chen", Phone = "0900000002" },
            new Card { Id = 3, Name = "Demo Fan", Phone = "0900000003" },
            new Card { Id = 4, Name = "Dino Wang", Phone = "0900000004" },
            new Card { Id = 5, Name = "Jerry Chiang", Phone = "0900000005" },
            new Card { Id = 6, Name = "Kevin Tseng", Phone = "0900000006" },
            new Card { Id = 7, Name = "Wade Huang", Phone = "0900000007" },
            new Card { Id = 8, Name = "阿砮", Phone = "0900000008" },
        };

        // GET: Card
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View(Cards);
        }


        public ActionResult Create()
        {
            var entity = new Card();

            return View("Edit", entity);
        }

        [HttpPost]
        public ActionResult Create(Card card)
        {
            var id = Cards.Max(x => x.Id);

            card.Id = id + 1;

            Cards.Add(card);

            TriggerContext.Current.Parent(x => x.RaiseEvent("body", "reload-table", null).FancyClose());

            return View("Edit");
        }

        public ActionResult Edit(int id)
        {
            var entity = Cards.First(x => x.Id == id);

            return View(entity);
        }

        [HttpPost]
        public ActionResult Edit(Card card)
        {
            var entity = Cards.First(x => x.Id == card.Id);

            entity.Name = card.Name;
            entity.Phone = card.Phone;

            TriggerContext.Current.Parent(x => x.RaiseEvent("body", "reload-table", null).FancyClose());

            return View();
        }
    }
}