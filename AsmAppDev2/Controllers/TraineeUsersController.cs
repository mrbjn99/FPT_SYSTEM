using AsmAppDev2.Models;
using AsmAppDev2.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;


namespace AsmAppDev2.Controllers
{
	public class TraineeUsersController : Controller
	{
		private ApplicationDbContext _context;
		public TraineeUsersController()
		{
			_context = new ApplicationDbContext();
		}



		// GET: Trainees
		[HttpGet]
		[Authorize(Roles = "Staff , Trainee")]
		public ActionResult Index(string searchString)
		{
			var trainee = _context.TraineeUsers.Include(te => te.Trainee);
			if (!String.IsNullOrEmpty(searchString))
			{
				trainee = trainee.Where(s => s.Trainee.UserName.Contains(searchString));
				return View(trainee);
			}

			if (User.IsInRole("Staff"))
			{
				var viewTrainee = _context.TraineeUsers.Include(a => a.Trainee).ToList();
				return View(viewTrainee);
			}
			if (User.IsInRole("Trainee"))
			{
				var traineeId = User.Identity.GetUserId();
				var traineeVM = _context.TraineeUsers.Where(te => te.TraineeID == traineeId).ToList();
				return View(traineeVM);
			}
			return View("Index");
		}

		[HttpGet]
		[Authorize(Roles = "Staff")]
		public ActionResult Create()
		{
			//Get Account Trainee
			var userInDb = (from r in _context.Roles where r.Name.Contains("Trainee") select r).FirstOrDefault();
			var users = _context.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(userInDb.Id)).ToList();
			var traineeUser = _context.TraineeUsers.ToList();
			var viewModel = new TraineeUserViewModel
			{
				Trainees = users,
				TraineeUser = new TraineeUser()
			};
			return View(viewModel);
		}


		[HttpPost]
		[Authorize(Roles = "Staff")]
		public ActionResult Create(TraineeUserViewModel trainee)
		{
			var traineeinDb = (from te in _context.Roles where te.Name.Contains("Trainee") select te).FirstOrDefault();
			var traineeUser = _context.Users.Where(u => u.Roles.Select(us => us.RoleId).Contains(traineeinDb.Id)).ToList();
			if (ModelState.IsValid)
			{

				var checkTraineeExist = _context.TraineeUsers.Include(t => t.Trainee).Where(t => t.Trainee.Id == trainee.TraineeUser.TraineeID);
				//GET TraineeID 
				if (checkTraineeExist.Count() > 0)  //list ID comparison, if count == 0. jump to else
				// if (checkTraineeExist.Any())
				{
					ModelState.AddModelError("", "Trainee Already Exists.");
				}
				else
				{
					_context.TraineeUsers.Add(trainee.TraineeUser);
					_context.SaveChanges();
					return RedirectToAction("Index");
				}
			}
			TraineeUserViewModel traineeUserView = new TraineeUserViewModel()
			{
				Trainees = traineeUser,
				TraineeUser = trainee.TraineeUser
			};
			return View(traineeUserView);
		}



		//[HttpPost]
		//[Authorize(Roles = "Staff")]
		//public ActionResult Create(TraineeUser user)
		//{ // Check the value of Id
		//	if (!ModelState.IsValid)
		//	{
		//		return View();
		//	}
		//	var usernameExist = _context.Users.Any(u => u.UserName.Contains(user.TraineeID));
		//	//  var EmailIsExist = _context.Users.Any(u => u.Email.Contains(user.Email));
		//	if (usernameExist)
		//	{
		//		ModelState.AddModelError("ID", "Trainee existed");
		//		return View();
		//	}
		//	var userInDb = _context.Users.SingleOrDefault(u => u.Email == user.Email);

		//	if (userInDb != null)
		//	{
		//		return HttpNotFound();
		//	}
		//	_context.TraineeUsers.Add(user);
		//	_context.SaveChanges();
		//	return RedirectToAction("Index");
		//}




		[HttpGet]
		[Authorize(Roles = "Staff, Trainee")]
		public ActionResult Edit(int id)
		{
			var traineeInDb = _context.TraineeUsers.SingleOrDefault(te => te.ID == id);
			if (traineeInDb == null)
			{
				return HttpNotFound();
			}
			return View(traineeInDb);
		}

		[HttpPost]
		[Authorize(Roles = "Staff, Trainee")]
		public ActionResult Edit(TraineeUser traineeUser)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			var traineeInDb = _context.TraineeUsers.SingleOrDefault(te => te.ID == traineeUser.ID);
			if (traineeInDb == null)
			{
				return HttpNotFound();
			}
			traineeInDb.Email = traineeUser.Email;
			traineeInDb.Full_Name = traineeUser.Full_Name;
			traineeInDb.Education = traineeUser.Education;
			traineeInDb.Programming_Language = traineeUser.Programming_Language;
			traineeInDb.Experience_Details = traineeUser.Experience_Details;
			traineeInDb.Department = traineeUser.Department;
			//traineeInDb.Phone = traineeUser.Phone;
			_context.SaveChanges();
			return RedirectToAction("Index");
		}


		[HttpGet]
		[Authorize(Roles = "Staff")]
		public ActionResult Delete(int id)
		{
			var traineeInDb = _context.TraineeUsers.SingleOrDefault(te => te.ID == id);
			if (traineeInDb == null)
			{
				return HttpNotFound();
			}
			_context.TraineeUsers.Remove(traineeInDb);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}