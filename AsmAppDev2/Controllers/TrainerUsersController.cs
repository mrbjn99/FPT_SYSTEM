using AsmAppDev2.Models;
using AsmAppDev2.ViewModels;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace AsmAppDev2.Controllers
{
	public class TrainerUsersController : Controller
	{
		private ApplicationDbContext _context;
		public TrainerUsersController()
		{
			_context = new ApplicationDbContext();
		}

		// GET: Trainees
		[HttpGet]
		[Authorize(Roles = "Staff , Trainer")]

		public ActionResult Index()
		{
			if (User.IsInRole("Staff"))
			{
				var viewTrainer = _context.TrainerUsers.Include(a => a.Trainer).ToList();
				return View(viewTrainer);
			}
			if (User.IsInRole("Trainer"))
			{
				var trainerId = User.Identity.GetUserId();
				var trainerVM = _context.TrainerUsers.Where(te => te.TrainerID == trainerId).ToList();
				return View(trainerVM);
			}
			return View("Index");
		}

		[HttpGet]
		[Authorize(Roles = "Staff")]
		public ActionResult Create()
		{
			//Get Account Trainer
			var userInDb = (from r in _context.Roles where r.Name.Contains("Trainer") select r).FirstOrDefault();
			var users = _context.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(userInDb.Id)).ToList();
			var trainerUser = _context.TrainerUsers.ToList();

			var viewModel = new TrainerUserViewModel
			{
				Trainers = users,
				TrainerUser = new TrainerUser()
			};
			return View(viewModel);
		}


		[HttpPost]
		[Authorize(Roles = "Staff")]
		public ActionResult Create(TrainerUserViewModel trainer)
		{
			var trainerinDb = (from te in _context.Roles where te.Name.Contains("Trainer") select te).FirstOrDefault();
			var trainerUser = _context.Users.Where(u => u.Roles.Select(us => us.RoleId).Contains(trainerinDb.Id)).ToList();
			if (ModelState.IsValid)
			{

				var checkTrainerExist = _context.TrainerUsers.Include(t => t.Trainer).Where(t => t.Trainer.Id == trainer.TrainerUser.TrainerID);
				if (checkTrainerExist.Count() > 0) //list ID comparison, if count == 0. jump to else
				{
					ModelState.AddModelError("", "Trainer Already Exists.");
				}
				else
				{
					_context.TrainerUsers.Add(trainer.TrainerUser);
					_context.SaveChanges();
					return RedirectToAction("Index");
				}
			}
			TrainerUserViewModel trainerUserView = new TrainerUserViewModel()
			{
				Trainers = trainerUser,
				TrainerUser = trainer.TrainerUser
			};
			return View(trainerUserView);
		}


		[HttpGet]
		[Authorize(Roles = "Staff, Trainer")]
		public ActionResult Edit(int id)
		{
			var trainerInDb = _context.TrainerUsers.SingleOrDefault(tn => tn.ID == id);
			if (trainerInDb == null)
			{
				return HttpNotFound();
			}
			return View(trainerInDb);
		}

		[HttpPost]
		[Authorize(Roles = "Staff, Trainer")]
		public ActionResult Edit(TrainerUser trainerUser)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			var trainerInDb = _context.TrainerUsers.SingleOrDefault(tn => tn.ID == trainerUser.ID);
			if (trainerInDb == null)
			{
				return HttpNotFound();
			}
			trainerInDb.TrainerID = trainerUser.TrainerID;
			trainerInDb.Full_Name = trainerUser.Full_Name;
			trainerInDb.Working_Place = trainerUser.Working_Place;
			trainerInDb.Phone = trainerUser.Phone;

			_context.SaveChanges();
			return RedirectToAction("Index");
		}



		[HttpGet]
		[Authorize(Roles = "Staff")]
		public ActionResult Delete(int id)
		{
			var trainerInDb = _context.TrainerUsers.SingleOrDefault(tn => tn.ID == id);
			if (trainerInDb == null)
			{
				return HttpNotFound();
			}
			_context.TrainerUsers.Remove(trainerInDb);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}