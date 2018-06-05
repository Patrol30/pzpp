using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DataBase;
using PZPP_Web.Models;

namespace PZPP_Web.Controllers
{
    [Authorize]
    public class PingController : Controller
    {
        // GET: Ping
        public ActionResult Index()
        {
            var model = new PingViewModel();
            var d = new List<SelectListItem>();
            using (var db = new PingDataContext())
            {

                foreach (var item in db.Devices)
                {

                    d.Add(new SelectListItem() { Text = item.IP + " " + item.Description });

                }
                model.DropDown = d;
            }
            return View(model);
        }

        public ActionResult Responses()
        {
            List<ResponseModel> model = new List<ResponseModel>();
            using (var db = new PingDataContext())
            {
                foreach (var item in db.Responses)
                {
                    model.Add(new ResponseModel() {
                        Id =item.Id,
                        Device_Id= item.Device_Id,
                        Success= item.Success,
                        PingTime=item.PingTime,
                        Time=item.Time
                    });                    
                }
            }
            return View(model);
        }
        public ActionResult Devices()
        {
            List<DeviceModel> model = new List<DeviceModel>();
            using (var db = new PingDataContext())
            {
                foreach (var item in db.Devices)
                {
                    model.Add(new DeviceModel()
                    {
                        Id = item.Id,
                        IP = item.IP,
                        Description = item.Description

                    });
                }
            }
            return View(model);
        }
        [Authorize(Roles = "Moderator")]
        public ActionResult AddDevice(PingViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new PingDataContext())
                {

                    db.Devices.Add(new Devices() { IP = model.IP, Description = model.Description });
                    db.SaveChanges();

                }
                return RedirectToAction("Index");

            }

            // If we got this far, something failed, redisplay form
            return View("Index", model);

        }
        [Authorize(Roles = "Admin")]
        public ActionResult PingDevice(PingViewModel model)
        {
            
            Ping pinger = new Ping();
            try
            {
                string data = model.Selected.First().ToString();
                string address = data.Remove(data.IndexOf(" "));
                // var id = IdControl[_SelectedDevice];//bierze id zaznaczonego urządzenia 
                PingReply reply = pinger.Send(address);//przypisanie jakie ip pingujemy 
                using (var db = new PingDataContext())
                {
                    if (reply.Status == IPStatus.Success)//jeśli ping się powiódł 
                    {

                        db.Responses.Add(new Responses() //sapisanie donych do bazy
                        {
                            Device_Id = db.Devices.Where(x => x.IP == address).Where(x => data.Contains(x.Description)).First().Id,
                            Success = true,
                            PingTime = reply.RoundtripTime,//czas pingu (ms)
                            Time = DateTime.Now//Data pingu
                        });
                    }
                    else//jak się nie udał
                    {

                        db.Responses.Add(new Responses()
                        {

                            Device_Id = db.Devices.Where(x => x.IP == address).Where(x => data.Contains(x.Description)).First().Id,
                            Success = false,
                            Time = DateTime.Now


                        });
                    }
                    db.SaveChanges(); //zapis danych do bazy (zostaje przesłane do bazy SQL)
                }
             
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }           
                        
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteR(int id)
        {

            using (var db = new PingDataContext())
            {
                Responses Response = new Responses() { Id = id };
                db.Responses.Attach(Response);
                db.Responses.Remove(Response);
                db.SaveChanges(); //zapis danych do bazy (zostaje przesłane do bazy SQL)
            }
            return RedirectToAction("Responses");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {

            using (var db = new PingDataContext())
            {
                Devices Device = new Devices() { Id = id };
                db.Devices.Attach(Device);
                db.Devices.Remove(Device);
                db.SaveChanges(); //zapis danych do bazy (zostaje przesłane do bazy SQL)
            }
            return RedirectToAction("Devices");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            DeviceModel model = new DeviceModel();
            using (var db = new PingDataContext())
            {
                model.Id = id;
                model.IP = db.Devices.Where(x => x.Id == id).First().IP;
                model.Description = db.Devices.Where(x => x.Id == id).First().Description;

            }
            return View("EditDevice",model);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult EditDevice(DeviceModel model)
        {
            using (var db = new PingDataContext())
            {
                db.Devices.Where(x => x.Id == model.Id).First().IP = model.IP;
                db.Devices.Where(x => x.Id == model.Id).First().Description = model.Description;
                db.SaveChanges();
            }
            return RedirectToAction("Devices");
        }
    }
}





