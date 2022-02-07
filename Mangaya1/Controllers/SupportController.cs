using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mangaya1.Models;

namespace Mangaya1.Controllers
{
    public class SupportController : Controller
    {
        // GET: Support
        mangayaEntities db = new mangayaEntities();
        public ActionResult home()
        {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            Session["supportid"] = 1;
            return View();
        }
        [HttpGet]
        public ActionResult login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult login(string username,string password)
        {

            support s = db.supports.Where(x => x.user_name.ToLower().Equals(username.ToLower()) && x.password.Equals(password)).FirstOrDefault();
            if(s==null)
            {
                ViewBag.wronglogin = "faild";
                return View();
            }
            else
            {
                if (s.position == 0)//transelator
                {
                    Session["supportid"] = s.id;
                    return Redirect("/Support/home");
                }
                else if (s.position==1)//media member
                {
                    Session["supportid"] = s.id;
                    return Redirect("/Media/home");
                }                   
                else
                {
                    Session["supportid"] = s.id;
                    return Redirect("/Manager/home");
                }

            }           
        }
        public ActionResult profiledata(int supportid)
        {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            support s = db.supports.Find(supportid);
            int totalviews;
            int totaldownloads;
            int totalviewsthismonth;
            int totaldownloadsthismonth;
            if(s.position==1)
            {
                s.first_language = "";
                s.second_language = "";
                totaldownloads = 0;
                totaldownloadsthismonth = 0;
                totalviews = db.voiceoverchapters.Where(x => x.voiceoverid == s.id).Sum(y=> (int?)y.chapter.no_of_views)??0;
                totalviewsthismonth = db.voiceoverchapters.Where(x => x.voiceoverid == s.id &&x.chapter.date.Month==DateTime.Now.Month&& x.chapter.date.Year == DateTime.Now.Year).Sum(y => (int?)y.chapter.no_of_views) ?? 0;
            }
            else
            {
                totalviews = db.translations.Where(x => x.tanslator_id.Equals(supportid)).Select(x => (int?)x.chapter.no_of_views).Sum() ?? 0;
                totaldownloads = db.translations.Where(x => x.tanslator_id.Equals(supportid)).Select(x => (int?)x.chapter.no_of_downloads).Sum() ?? 0;
                totalviewsthismonth = db.translations.Where(x => x.tanslator_id.Equals(supportid) && x.chapter.date.Month == DateTime.Now.Month && x.chapter.date.Year == DateTime.Now.Year).Select(x => (int?)x.chapter.no_of_views).Sum() ?? 0;
                totaldownloadsthismonth = db.translations.Where(x => x.tanslator_id.Equals(supportid) && x.chapter.date.Month == DateTime.Now.Month && x.chapter.date.Year == DateTime.Now.Year).Select(x => (int?)x.chapter.no_of_downloads).Sum() ?? 0;
             
            }
            return Json(new {
                supportName = s.name,
                supportUserame = s.user_name,
                supportPassword = s.password,
                supportEmail = s.email,
                supportFirstLanguage = s.first_language,
                supportSecondLanguage = s.second_language,
                supportPhone = s.phone,
                totalviews = totalviews,
                totaldownloads= totaldownloads,
                totalviewsthismonth= totalviewsthismonth,
                totaldownloadsthismonth= totaldownloadsthismonth,


            }, JsonRequestBehavior.AllowGet);
            
        }
        public ActionResult editprofilesupportdata(string supportName,string supportUserame,string supportPassword,string supportEmail,string supportFirstLanguage,string supportSecondLanguage,string supportPhone)
        {
            if (Session["supportid"] == null)
                return Redirect("/Support/login");
            support s = db.supports.Find((int)Session["supportid"]);
            s.name = supportName;
            s.user_name = supportUserame;
            s.password = supportPassword;
            s.email = supportEmail;
            s.first_language = supportFirstLanguage;
            s.second_language = supportSecondLanguage;
            s.phone = supportPhone;
            db.Entry(s).State = System.Data.Entity.EntityState.Modified;
           // db.supports.Attach(s);
            db.SaveChanges();
            return Json(new {code =1 }, JsonRequestBehavior.AllowGet);


        }
        public ActionResult showmytranslation(int supportid)
        {   
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            support s = db.supports.Find(supportid);
            List<manga> mytransilations = db.translations.Where(x => x.tanslator_id == s.id).Select(x => x.manga).Distinct().ToList();
            List<manga> doesnotbelongtoanyone = db.mangas.Where(x => x.chapters.Count == 0).ToList();
            List<object> result = new List<object>();
            mytransilations.AddRange(doesnotbelongtoanyone);
            foreach (var item in mytransilations)
            {
                string rate = "";
                if (!item.rate.ToString().Contains('.'))
                    rate = item.rate.ToString() + ".0";
                else
                    rate = item.rate.ToString();

                result.Add( new{
                    manganameENG=item.name_ENG,
                    manganameARB=item.name_ARB,
                    mangarate= rate,
                    mangaid=item.id,
                    mangaimg=item.profile_pic
                });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult editmangaveiw(int supportid,int mangaid)
            {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            manga m = db.mangas.Find(mangaid);
            string rate = "";
            if (!m.rate.ToString().Contains('.'))
                rate = m.rate.ToString() + ".0";
            else
                rate = m.rate.ToString();
            List<object> chs = new List<object>();
            List<chapter> mangachs = db.chapters.Where(x => x.manga_id == mangaid).ToList();
            foreach (var item in mangachs)
            {
                string chaptrate = "";
                if (!item.rate.ToString().Contains('.'))
                    chaptrate = item.rate.ToString() + ".0";
                else
                    chaptrate = item.rate.ToString();
               
                chs.Add(new {

                    chapterarte = chaptrate,
                    chaptername = item.chaNameARB+"\n"+item.chaNameENG,
                    chapternumber = item.chapter_number,
                    chapterid = db.chapters.Where(y => y.manga_id == item.manga_id && y.chapter_number == item.chapter_number).Select(v => v.id).FirstOrDefault()
                   
                });
            }
            List<string> categories = m.category_manga.Select(cat=>cat.category_type.type_arb).ToList();
            string cc = "";
            foreach (var item in categories)
            {
                cc += item;
                cc += " - ";
            }
            char[] s = cc.ToArray();
            if (s.Length > 2) 
           s[s.Length - 2] = ' ';
            cc = new string(s);
            List<object> catsb = new List<object>();
            List<category_type> cats = db.category_type.ToList();
            foreach (var item in cats)
            {
                catsb.Add(new {
                  id= item.categoryid,
                  type=  item.type_arb
                });
            }

            return Json(new {

                editmangaveiwmanganame = m.name_ENG + "/" + m.name_ARB,
                editmangaveiwnameENG=m.name_ENG,
                editmangaveiwnameARB= m.name_ARB,
                editmangaveiwno_chapters = m.no_of_chapters,
                editmangaveiwcontry = m.Country,
                editmangaveiwdecription = m.decription,
                editmangaveiwcategories = cc,
                editmangaveiwrate = rate,
                editmangaveiwimg = m.profile_pic,
                chapters = chs,
                categorylist= categories,
                categoryoption= catsb
            }, JsonRequestBehavior.AllowGet);


        }
        public ActionResult AddnewManaga(int supportid, string editdata)
        {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            string[] vv = editdata.Split(',');

            manga m = new manga();
            m.name_ENG = vv[1];
            m.name_ARB = vv[2];
            m.rate = float.Parse(vv[3]);
            m.no_of_chapters = int.Parse(vv[4]);
            m.Country = vv[5];
            m.profile_pic = vv[6];
            m.decription = vv[7];
            db.mangas.Add(m);
            db.SaveChanges();
            for (int i = 8; i < vv.Length; i++)
            {
                string category = vv[i];
                category_type s = db.category_type.Where(x => x.type_arb.Equals(category)).FirstOrDefault();
                category_manga rec = new category_manga();
                rec.category_id = s.categoryid;
                rec.manga_id = m.id;
                db.category_manga.Add(rec);
            }
            db.SaveChanges();
            return Json(new { code = 1 }, JsonRequestBehavior.AllowGet);
            
        }
        public ActionResult updatemangainfo(int supportid , string editdata)
        {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            string[] vv = editdata.Split(',');          
            manga m = db.mangas.Find(int.Parse(vv[0]));
            m.name_ENG = vv[1];
            m.name_ARB = vv[2];
            m.rate = float.Parse(vv[3]);
            m.no_of_chapters = int.Parse(vv[4]);
            m.Country = vv[5];
            m.profile_pic = vv[6];
            m.decription = vv[7];
            List<category_manga> mc = db.category_manga.Where(c => c.manga_id == m.id).ToList();
            db.category_manga.RemoveRange(mc);
            for (int i = 8; i < vv.Length; i++)
            {
                string category = vv[i];
                category_type s = db.category_type.Where(x => x.type_arb.Equals(category)).FirstOrDefault();             
                category_manga rec = new category_manga();
                rec.category_id = s.categoryid;
                rec.manga_id = m.id;
                db.category_manga.Add(rec);        
            }
            db.Entry(m).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(new { code = 1 }, JsonRequestBehavior.AllowGet);
            
        }
        public ActionResult deletemanga(int supportid, int mangaid)
        {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            manga m = db.mangas.Find(mangaid);
            if (m != null)
            {
             List<category_manga>cm=  db.category_manga.Where(x => x.manga_id == m.id).ToList();
                db.category_manga.RemoveRange(cm);
                db.mangas.Remove(m);
                db.SaveChanges();
                return Json(new { code = 1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { code = 0 }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddnewChapter(int supportid,string Data)
        {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            support s = db.supports.Find((int)Session["supportid"]);
            string[] vv = Data.Split(',');     
            chapter c = new chapter();
           
            c.chaNameARB = vv[0];
            c.chaNameENG = vv[1];
            c.chapter_number = int.Parse(vv[2]);
            c.rate = float.Parse(vv[3]);
            c.manga_id = int.Parse(vv[4]);
            c.no_of_downloads = 0;
            c.no_of_views = 0;
            c.total_profit = 0;
            c.date = DateTime.Now;
            db.chapters.Add(c);
            db.SaveChanges();
            c = db.chapters.Where(x => x.chapter_number == c.chapter_number && x.manga_id == c.manga_id).FirstOrDefault();
            translation t = new translation();
            t.manga_id = c.manga_id;
            t.chapter_id = c.id;
            t.tanslator_id = s.id;
            db.translations.Add(t);
            db.SaveChanges();

            for (int i = 5; i < vv.Length-1; i+=2)
            {
                chapter_content ch_c = new chapter_content();
                ch_c.chapter_id = c.id;
                if (vv[i] == "")
                    continue;
                ch_c.slide_number = int.Parse(vv[i]);
                ch_c.slide_content = vv[i + 1];
                db.chapter_content.Add(ch_c);
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
            List<chapter_content> ch_content = db.chapter_content.Where(v => v.chapter_id == c.id).ToList();
            List<object> ch_ob_contnet = new List<object>();
            foreach (var item in ch_content)
            {
                ch_ob_contnet.Add(new {
                    id=item.id,
                    slidenumber=item.slide_number,
                    sidecontnet=item.slide_content
                });
            }
            return Json(new {
               chaptemanganame = c.manga.name_ARB + " - " + c.manga.name_ENG,
                 chapterimg = c.manga.profile_pic,
                chaterno =c.chapter_number,
                chapterrate= chaptrate,
                chaptertitle=c.chaNameARB+" - "+c.chaNameENG,
                chaptercountry =c.manga.Country,
                chapterdescription = c.manga.decription,
                chaptercategories= categoriesd,
                chapter_contnent= ch_ob_contnet                
            }, JsonRequestBehavior.AllowGet);


        }
        public ActionResult viewchapter(int supportid,int chapterid)
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
            List<chapter_content> ch_content = db.chapter_content.Where(v => v.chapter_id == c.id).ToList();
            List<object> ch_ob_contnet = new List<object>();
            foreach (var item in ch_content)
            {
                ch_ob_contnet.Add(new
                {
                    id = item.id,
                    slidenumber = item.slide_number,
                    sidecontnet = item.slide_content
                });
            }

            return Json(new {
                chaptemanganame = c.manga.name_ARB + " - " + c.manga.name_ENG,
                chapterNameARB= c.chaNameARB,
                chapterNameENG= c.chaNameENG,
                chapterimg = c.manga.profile_pic,
                chaterno = c.chapter_number,
                chapterrate = chaptrate,
                chaptertitle = c.chaNameARB + " - " + c.chaNameENG,
                chaptercountry = c.manga.Country,
                chapterdescription = c.manga.decription,
                chaptercategories = categoriesd,
                chapter_contnent = ch_ob_contnet

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult editchapter(int supportid, string Data)
        {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            support s = db.supports.Find((int)Session["supportid"]);
            string[] vv = Data.Split(',');
            int chapterno = int.Parse(vv[2]);
            int m_id = int.Parse(vv[4]);
            chapter c = db.chapters.Where(x => x.chapter_number == chapterno && x.manga_id == m_id).FirstOrDefault();
            c.chaNameARB = vv[0];
            c.chaNameENG = vv[1];
            c.chapter_number = int.Parse(vv[2]);
            c.rate = float.Parse(vv[3]);
            c.manga_id = int.Parse(vv[4]);
            c.no_of_downloads = 0;
            c.no_of_views = 0;
            c.total_profit = 0;
            c.date = DateTime.Now;
            db.Entry(c).State= System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            c = db.chapters.Where(x => x.chapter_number == c.chapter_number && x.manga_id == c.manga_id).FirstOrDefault();
           
            for (int i = 5; i < vv.Length - 1; i += 3)
            {
                int id;
                if (vv[i] == "") {
                    id = 0;
                }
                else
                {
                    id = int.Parse(vv[i]);
                }
                chapter_content ch_c = db.chapter_content.Find(id);
                if(ch_c==null)
                {
                    ch_c = new chapter_content();
                    ch_c.chapter_id = c.id;
                    if (vv[i+1] == "")
                        continue;
                    ch_c.slide_number = int.Parse(vv[i+1]);
                    ch_c.slide_content = vv[i + 2];
                    db.chapter_content.Add(ch_c);
                }
                else
                {
                    ch_c.chapter_id = c.id;
                    if (vv[i] == "")
                        continue;
                        ch_c.slide_number = int.Parse(vv[i+1]);
                    ch_c.slide_content = vv[i + 2];
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
            List<chapter_content> ch_content = db.chapter_content.Where(v => v.chapter_id == c.id).ToList();
            List<object> ch_ob_contnet = new List<object>();
            foreach (var item in ch_content)
            {
                ch_ob_contnet.Add(new
                {
                    id = item.id,
                    slidenumber = item.slide_number,
                    sidecontnet = item.slide_content
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
                chapterNameARB= c.chaNameARB,
                chapterNameENG= c.chaNameENG

            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult editchaptercontentlink(string editdata)
        {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            string[] vv = editdata.Split(',');
            int id = int.Parse(vv[0]);
            int slide_no = int.Parse(vv[1]);
            chapter_content ch = db.chapter_content.Find(id);
            ch.slide_number = slide_no;
            ch.slide_content = vv[2];
            db.Entry(ch).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult deletechaptercontentlink (string editdata)
        {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            string[] vv = editdata.Split(',');
            int id = int.Parse(vv[0]);
            chapter_content ch = db.chapter_content.Find(id);
            db.chapter_content.Remove(ch);
            db.SaveChanges();
            return Json(new { }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult deletechapter(int chapterid)
        {
            //if (Session["supportid"] == null)
            //    return Redirect("/Support/login");
            support s = db.supports.Find((int)Session["supportid"]);
            List<translation> ts = db.translations.Where(x => x.chapter_id == chapterid).ToList();
            db.translations.RemoveRange(ts);
            List<chapter_content> cc = db.chapter_content.Where(x => x.chapter_id == chapterid).ToList();
            db.chapter_content.RemoveRange(cc);
            chapter c = db.chapters.Find(chapterid);
            db.chapters.Remove(c);
            db.SaveChanges();
            return Json(new { }, JsonRequestBehavior.AllowGet);


        }
        public ActionResult reviewchapter(int chapterid)
        {
            chapter c = db.chapters.Find(chapterid);
            if (c == null)
                return Json(new { },JsonRequestBehavior.AllowGet);

            List<object> data = new List<object>();
            List<chapter_content> ss = c.chapter_content.ToList();
            foreach (var item in ss)
            {
                data.Add(new {

                    slideid = item.id,
                    slidenumber=item.slide_number,
                    slidecontent=item.slide_content
                });
            }
            return Json(new {result = data }, JsonRequestBehavior.AllowGet);

        }
    }
}