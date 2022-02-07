using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mangaya1
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            

            routes.MapRoute(
                    "profiledata",
                    "Support/profiledata/{supportid}",
                    new { Controller = "Support", action = "profiledata" });

            routes.MapRoute(
                    "ditprofilesupportdata",
                    "Support/ditprofilesupportdata/{supportName}/{supportUserame}/{supportPassword}/{supportEmail}/{supportFirstLanguage}/{supportSecondLanguage}/{supportPhone}",
                    new { Controller = "Support", action = "ditprofilesupportdata" });

            routes.MapRoute(
              "showmytranslation",
              "Support/showmytranslation/{supportid}",
              new { Controller = "Support", action = "showmytranslation" });
            
            routes.MapRoute(
              "editmangaveiw",
              "Support/editmangaveiw/{supportid}/{mangaid}",
              new { Controller = "Support", action = "editmangaveiw" });

            routes.MapRoute(
              "updatemangainfo",
              "Support/updatemangainfo/{supportid}/{editdata}",
              new { Controller = "Support", action = "updatemangainfo" });

            routes.MapRoute(
                      "deletemanga",
                           "Support/deletemanga/{supportid}/{mangaid}",
                      new { Controller = "Support", action = "deletemanga" });
         

            routes.MapRoute(
                      "AddnewChapter",
                           "Support/AddnewChapter/{supportid}/{Data}",
                      new { Controller = "Support", action = "AddnewChapter" });
            routes.MapRoute(
                      "viewchapter",
                           "Support/viewchapter/{supportid}/{chapterid}",
                      new { Controller = "Support", action = "viewchapter" });

            routes.MapRoute(
                     "editchapter",
               "Support/editchapter/{supportid}/{Data}",
          new { Controller = "Support", action = "editchapter" });
            
                   routes.MapRoute(
                     "deletechaptercontentlink",
               "Support/deletechaptercontentlink/{editdata}",
          new { Controller = "Support", action = "deletechaptercontentlink" });

            routes.MapRoute(
                     "editchaptercontentlink",
               "Support/editchaptercontentlink/{editdata}",
          new { Controller = "Support", action = "editchaptercontentlink" });
       
            routes.MapRoute(
                     "deletechapter",
               "Support/deletechapter/{chapterid}",
          new { Controller = "Support", action = "deletechapter" });
         

            routes.MapRoute(
                     "reviewchapter",
               "Support/reviewchapter/{chapterid}",
          new { Controller = "Support", action = "reviewchapter" });


            routes.MapRoute(
                     "ShowAllMangas",
               "Media/ShowAllMangas/{supportid}",
          new { Controller = "Media", action = "ShowAllMangas" });
       
            routes.MapRoute(
                     "viewchapter1",
               "Media/viewchapter/{chapterid}",
          new { Controller = "Media", action = "viewchapter" });
            routes.MapRoute(
                    "AddnewActortochapter",
              "Media/AddnewActortochapter/{chapterid}/{actorid}",
         new { Controller = "Media", action = "AddnewActortochapter" });
            routes.MapRoute(
                 "deleteactorfromcahpter",
           "Media/deleteactorfromcahpter/{chapterid}/{actorid}",
        new { Controller = "Media", action = "deleteactorfromcahpter" });
                    routes.MapRoute(
                   "editchaptervoices",
             "Media/editchaptervoices/{Data}",
        new { Controller = "Media", action = "editchaptervoices" });
                    routes.MapRoute(
               "editchaptervoicelink",
         "Media/editchaptervoicelink/{editdata}",
        new { Controller = "Media", action = "editchaptervoicelink" });
            routes.MapRoute(
             "deletechaptervoicelink",
       "Media/deletechaptervoicelink/{editdata}",
      new { Controller = "Media", action = "deletechaptervoicelink" });
            routes.MapRoute(
            "reviewchapterwithvoice",
      "Media/reviewchapter/{chapterid}",
 new { Controller = "Media", action = "reviewchapter" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
