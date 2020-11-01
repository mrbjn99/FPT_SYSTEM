using AsmAppDev2.Models;
using AsmAppDev2.ViewModels;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace AsmAppDev2.Controllers
{
	public class AssignTraineetoCoursesController : Controller
	{
		private ApplicationDbContext _context;
		public AssignTraineetoCoursesController()
		{
			_context = new ApplicationDbContext();
		}

		[HttpGet]
		[Authorize(Roles = "Staff, Trainee")]
		// GET: AssignTraineetoCourses
		public ActionResult Index()
		{
			if (User.IsInRole("Staff"))
			{
				var viewAssign = _context.AssignTraineetoCourses.Include(a => a.Course).Include(a => a.Trainee).ToList();
				return View(viewAssign);
			}
			if (User.IsInRole("Trainee"))
			{
				var traineeId = User.Identity.GetUserId();
				var traineeVM = _context.AssignTraineetoCourses.Where(te => te.TraineeID == traineeId).Include(te => te.Course).ToList();
				return View(traineeVM);
			}
			return View();
		}

		//GET: Trainee and Course
		[HttpGet]
		[Authorize(Roles = "Staff")]
		public ActionResult Create()
		{
			var traineeInDb = (from te in _context.Roles where te.Name.Contains("Trainee") select te).FirstOrDefault();
			// Get User role name Trainee and return
			var traineeUser = _context.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(traineeInDb.Id)).ToList();
			// Get User in table and select the ID containing the TraineeID
			var courses = _context.Courses.ToList();

			var viewModel = new AssignTraineetoCourseViewModel
			{
				Courses = courses,
				Trainees = traineeUser,
				AssignTraineetoCourse = new AssignTraineetoCourse()
			};
			return View(viewModel);
		}


		[HttpPost]
		[Authorize(Roles = "Staff")]
		public ActionResult Create(AssignTraineetoCourseViewModel assign)
		{
			var traineeInDb = (from te in _context.Roles where te.Name.Contains("Trainee") select te).FirstOrDefault();
			var traineeUser = _context.Users.Where(u => u.Roles.Select(us => us.RoleId).Contains(traineeInDb.Id)).ToList();
			var course = _context.Courses.ToList();

			if (ModelState.IsValid)
			{
				var checkTraineeAndCourseExist = _context.AssignTraineetoCourses.Include(t => t.Course).Include(t => t.Trainee)
					.Where(t => t.Course.ID == assign.AssignTraineetoCourse.CourseID && t.Trainee.Id == assign.AssignTraineetoCourse.TraineeID);
				//GET CourseID and TraineeID from the Course and Trainee tables in the ViewModel

				if (checkTraineeAndCourseExist.Count() > 0) //list ID comparison, if count == 0. jump to else
				{
					ModelState.AddModelError("", "Assign Already Exists");
				}
				else
				{
					_context.AssignTraineetoCourses.Add(assign.AssignTraineetoCourse);
					_context.SaveChanges();
					return RedirectToAction("Index");
				}
			}
			AssignTraineetoCourseViewModel traineecourseVM = new AssignTraineetoCourseViewModel()
			{
				Courses = course,
				Trainees = traineeUser,
				AssignTraineetoCourse = assign.AssignTraineetoCourse
			};
			return View(traineecourseVM);
		}


		[HttpGet]
		[Authorize(Roles = "Staff")]
		public ActionResult Delete(int id)
		{
			var assignInDb = _context.AssignTraineetoCourses.SingleOrDefault(a => a.ID == id);
			if (assignInDb == null)
			{
				return HttpNotFound();
			}

			_context.AssignTraineetoCourses.Remove(assignInDb);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}