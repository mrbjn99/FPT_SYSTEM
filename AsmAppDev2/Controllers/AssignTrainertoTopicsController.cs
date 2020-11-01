using AsmAppDev2.Models;
using AsmAppDev2.ViewModels;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace AsmAppDev2.Controllers
{
	public class AssignTrainertoTopicsController : Controller
	{
		private ApplicationDbContext _context;

		public AssignTrainertoTopicsController()
		{
			_context = new ApplicationDbContext();
		}

		[Authorize(Roles = "Staff, Trainer")]
		// GET: AssignTrainertoTopics
		[HttpGet]
		public ActionResult Index()
		{
			if (User.IsInRole("Staff"))
			{
				var viewAssign = _context.AssignTrainertoTopics.Include(a => a.Topic).Include(a => a.Trainer).ToList();
				return View(viewAssign);
			}
			if (User.IsInRole("Trainer"))
			{
				var trainerId = User.Identity.GetUserId();
				var trainerVM = _context.AssignTrainertoTopics.Where(te => te.TrainerID == trainerId).Include(te => te.Topic).ToList();
				return View(trainerVM);
			}
			return View();
		}

		//GET: Trainer and Topic
		[HttpGet]
		[Authorize(Roles = "Staff")]
		public ActionResult Create()
		{
			var trainerInDb = (from tn in _context.Roles where tn.Name.Contains("Trainer") select tn).FirstOrDefault();
			var trainerUser = _context.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(trainerInDb.Id)).ToList();
			var topics = _context.Topics.ToList();
			var viewModel = new AssignTrainertoTopicViewModel
			{
				Topics = topics,
				Trainers = trainerUser,
				AssignTrainertoTopic = new AssignTrainertoTopic()
			};
			return View(viewModel);
		}


		[HttpPost]
		[Authorize(Roles = "Staff")]
		public ActionResult Create(AssignTrainertoTopicViewModel assign)
		{
			var trainerInDb = (from tn in _context.Roles where tn.Name.Contains("Trainer") select tn).FirstOrDefault();
			var trainerUser = _context.Users.Where(u => u.Roles.Select(us => us.RoleId).Contains(trainerInDb.Id)).ToList();
			var topic = _context.Topics.ToList();
			if (ModelState.IsValid)
			{
				var checkTrainerAndTopicExist = _context.AssignTrainertoTopics.Include(t => t.Topic).Include(t => t.Trainer)
					.Where(t => t.Topic.ID == assign.AssignTrainertoTopic.TopicID && t.Trainer.Id == assign.AssignTrainertoTopic.TrainerID);

				if (checkTrainerAndTopicExist.Count() > 0) //list ID comparison, if count == 0. jump to else
				{
					ModelState.AddModelError("", "Assign Already Exists");
				}
				else
				{
					_context.AssignTrainertoTopics.Add(assign.AssignTrainertoTopic);
					_context.SaveChanges();
					return RedirectToAction("Index");
				}
			}

			AssignTrainertoTopicViewModel trainertopicVM = new AssignTrainertoTopicViewModel()
			{
				Topics = topic,
				Trainers = trainerUser,
				AssignTrainertoTopic = assign.AssignTrainertoTopic
			};
			return View(trainertopicVM);
		}

		[HttpGet]
		[Authorize(Roles = "Staff")]
		public ActionResult Delete(int id)
		{
			var assignInDb = _context.AssignTrainertoTopics.SingleOrDefault(a => a.ID == id);
			if (assignInDb == null)
			{
				return HttpNotFound();
			}

			_context.AssignTrainertoTopics.Remove(assignInDb);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}