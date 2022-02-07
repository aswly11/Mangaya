using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mangaya1.Models;
namespace Mangaya1.Controllers
{
    public class MediaController : Controller
    {
        mangayaEntities db = new mangayaEntities();
        // GET: Media
        public ActionResult home()
        {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            Session["supportid"] = 2;
            return View();
        }
        public ActionResult ShowAllMangas(int supportid)
        {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            support s = db.supports.Find(supportid);
            List<manga> allmangas = db.mangas.ToList();
            List<object> result = new List<object>();
            foreach (var item in allmangas)
            {
                string rate = "";
                if (!item.rate.ToString().Contains('.'))
                    rate = item.rate.ToString() + ".0";
                else
                    rate = item.rate.ToString();

                result.Add(new
                {
                    manganameENG = item.name_ENG,
                    manganameARB = item.name_ARB,
                    mangarate = rate,
                    mangaid = item.id,
                    mangaimg = item.profile_pic
                });
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public ActionResult viewchapter(int chapterid)
        {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            support s = db.supports.Find((int)Session["supportid"]);
            chapter c = db.chapters.Find(chapterid);
            c.manga = db.mangas.Find(c.manga_id);
            List<string> categories = c.manga.category_manga.Select(cat => cat.category_type.type_arb).ToList();
            string cc = "";
            foreach (var item in categories)
            {
                cc += item;
                cc += " - ";
            }
            char[] st = cc.ToArray();
            if (st.Length > 2)
                st[st.Length - 2] = ' ';
            string categoriesd = new string(st);

            string chaptrate = "";
            if (!c.rate.ToString().Contains('.'))
                chaptrate = c.rate.ToString() + ".0";
            else
                chaptrate = c.rate.ToString();
            List<voicecontent> ch_content = db.voicecontents.Where(v => v.chapterid == c.id).ToList();
            List<object> ch_ob_contnet = new List<object>();
            foreach (var item in ch_content)
            {
                ch_ob_contnet.Add(new
                {
                    id = item.id,
                    slidenumber = item.slide_no,
                    sidecontnet = item.voicelink
                });
            }

            List<support> ss = db.voiceoverchapters.Where(x => x.chapterid == chapterid).Select(x => x.support).ToList();
            List<object> actualactors = new List<object>();
            foreach (var item in ss)
            {
                actualactors.Add(new {
                    Actorid= item.id,
                    ActorName= item.name
                });
            }
            List<support> allacts = db.supports.Where(x=>x.position==1).ToList();
            List<object>allactors = new List<object>();
            foreach (var item in allacts)
            {
                allactors.Add(new
                {
                    Actorid = item.id,
                    ActorName = item.name
                });
            }

            return Json(new
            {
                chaptemanganame = c.manga.name_ARB + " - " + c.manga.name_ENG,
                chapterNameARB = c.chaNameARB,
                chapterNameENG = c.chaNameENG,
                chapterimg = c.manga.profile_pic,
                chaterno = c.chapter_number,
                chapterrate = chaptrate,
                chaptertitle = c.chaNameARB + " - " + c.chaNameENG,
                chaptercountry = c.manga.Country,
                chapterdescription = c.manga.decription,
                chaptercategories = categoriesd,
                chapter_contnent = ch_ob_contnet,
                ActualActors= actualactors,
                allactors= allactors


            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddnewActortochapter(int chapterid, int actorid )
        {

            voiceoverchapter vc = new voiceoverchapter();
            vc.chapterid = chapterid;
            vc.voiceoverid = actorid;
            db.voiceoverchapters.Add(vc);
            db.SaveChanges();
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult deleteactorfromcahpter(int chapterid, int actorid)
        {

            voiceoverchapter vc = db.voiceoverchapters.Where(x => x.chapterid == chapterid && x.voiceoverid == actorid).FirstOrDefault();
            db.voiceoverchapters.Remove(vc);
            db.SaveChanges();
            return Json(new { }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult editchaptervoices( string Data)
        {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            support s = db.supports.Find((int)Session["supportid"]);
            string[] vv = Data.Split(',');
            int chapterno = int.Parse(vv[1]);
            int m_id = int.Parse(vv[0]);
            chapter c = db.chapters.Where(x => x.id == chapterno && x.manga_id == m_id).FirstOrDefault();
        
            for (int i = 2; i < vv.Length - 1; i += 3)
            {
                int id;
                if (vv[i] == "")
                {
                    id = 0;
                }
                else
                {
                    id = int.Parse(vv[i]);
                }
                voicecontent ch_c = db.voicecontents.Find(id);
                if (ch_c == null)
                {
                    ch_c = new voicecontent();
                    ch_c.chapterid = c.id;
                    if (vv[i + 1] == "")
                        continue;
                    ch_c.slide_no = int.Parse(vv[i + 1]);
                    ch_c.voicelink = vv[i + 2];
                    db.voicecontents.Add(ch_c);
                }
                else
                {
                    ch_c.chapterid = c.id;
                    if (vv[i] == "")
                        continue;
                    ch_c.slide_no = int.Parse(vv[i + 1]);
                    ch_c.voicelink = vv[i + 2];
                    db.Entry(ch_c).State = System.Data.Entity.EntityState.Modified;

                }

                db.SaveChanges();

            }
            c.manga = db.mangas.Find(c.manga_id);
            List<string> categories = c.manga.category_manga.Select(cat => cat.category_type.type_arb).ToList();
            string cc = "";
            foreach (var item in categories)
            {
                cc += item;
                cc += " - ";
            }
            char[] st = cc.ToArray();
            if (st.Length > 2)
                st[st.Length - 2] = ' ';
            string categoriesd = new string(st);

            string chaptrate = "";
            if (!c.rate.ToString().Contains('.'))
                chaptrate = c.rate.ToString() + ".0";
            else
                chaptrate = c.rate.ToString();
            List<voicecontent> ch_content = db.voicecontents.Where(v => v.chapterid == c.id).ToList();
            List<object> ch_ob_contnet = new List<object>();
            foreach (var item in ch_content)
            {
                ch_ob_contnet.Add(new
                {
                    id = item.id,
                    slidenumber = item.slide_no,
                    voicelink = item.voicelink
                });
            }
            return Json(new
            {
                chaptemanganame = c.manga.name_ARB + " - " + c.manga.name_ENG,
                chapterimg = c.manga.profile_pic,
                chaterno = c.chapter_number,
                chapterrate = chaptrate,
                chaptertitle = c.chaNameARB + " - " + c.chaNameENG,
                chaptercountry = c.manga.Country,
                chapterdescription = c.manga.decription,
                chaptercategories = categoriesd,
                chapter_contnent = ch_ob_contnet,
                chapterNameARB = c.chaNameARB,
                chapterNameENG = c.chaNameENG

            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult editchaptervoicelink(string editdata)
        {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            string[] vv = editdata.Split(',');
            int id = int.Parse(vv[0]);
            int slide_no = int.Parse(vv[1]);
            voicecontent ch = db.voicecontents.Find(id);
            ch.slide_no = slide_no;
            ch.voicelink = vv[2];
            db.Entry(ch).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult deletechaptervoicelink(string editdata)
        {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            string[] vv = editdata.Split(',');
            int id = int.Parse(vv[0]);
            voicecontent ch = db.voicecontents.Find(id);
            db.voicecontents.Remove(ch);
            db.SaveChanges();
            return Json(new { }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult reviewchapter(int chapterid) {

            chapter c = db.chapters.Find(chapterid);
            if (c == null)
                return Json(new { }, JsonRequestBehavior.AllowGet);

            List<object> data = new List<object>();
            List<chapter_content> ss = c.chapter_content.ToList();
            foreach (var item in ss)
            {
                data.Add(new
                {

                    slideid = item.id,
                    slidenumber = item.slide_number,
                    slidecontent = item.slide_content,
                    slidevoice=db.voicecontents.Where(x=>x.chapterid==item.chapter_id&&x.slide_no==item.slide_number).Select(x=>x.voicelink).FirstOrDefault()

                });
            }
            return Json(new { result = data }, JsonRequestBehavior.AllowGet);



        }
    }
}