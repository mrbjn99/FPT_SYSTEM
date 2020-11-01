using AsmAppDev2.Models;
using AsmAppDev2.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace AsmAppDev2.Controllers
{
	public class TopicsController : Controller
	{
		private ApplicationDbContext _context;
		public TopicsController()
		{
			_context = new ApplicationDbContext();
		}

		//GET: Topics
		[HttpGet]
		[Authorize(Roles = "Staff")]
		public ActionResult Index(string search)
		{
			var topics = _context.Topics.Include(t => t.Course);
			if (!String.IsNullOrEmpty(search))
			{
				topics = topics.Where(
						s => s.Name.Contains(search) ||
						s.Course.Name.Contains(search));
			}
			return View(topics);
		}

		//GET: Create
		[HttpGet]
		[Authorize(Roles = "Staff")]
		public ActionResult Create()
		{
			var viewModel = new CourseTopicViewModel
			{
				Courses = _context.Courses.ToList(),
				Topic = new Topic()
			};
			return View(viewModel);
		}

		[HttpPost]
		[Authorize(Roles = "Staff")]
		public ActionResult Create(CourseTopicViewModel topcourse)
		{
			if (ModelState.IsValid)
			{
				var check = _context.Topics.Include(c => c.Course).Where(c => c.Name == topcourse.Topic.Name && c.CourseID == topcourse.Topic.CourseID);

				if (check.Count() > 0) //list ID comparison, if count == 0. jump to else
				{
					ModelState.AddModelError("", "Topic Already Exists.");
				}
				else
				{
					_context.Topics.Add(topcourse.Topic);
					_context.SaveChanges();
					return RedirectToAction("Index");
				}
			}

			var topicVM = new CourseTopicViewModel()
			{
				Courses = _context.Courses.ToList(),
				Topic = topcourse.Topic,
			};
			return View(topicVM);
		}

		//GET: Edit
		[HttpGet]
		[Authorize(Roles = "Staff")]
		public ActionResult Edit(int id)
		{
			var topicInDb = _context.Topics.SingleOrDefault(t => t.ID == id);

			if (topicInDb == null)
			{
				return HttpNotFound();
			}
			var viewModel = new CourseTopicViewModel
			{
				Topic = topicInDb,
				Courses = _context.Courses.ToList()
			};
			return View(viewModel);
		}

		[HttpPost]
		[Authorize(Roles = "Staff")]
		public ActionResult Edit(CourseTopicViewModel edit)
		{
			if (ModelState.IsValid)
			{
				var check = _context.Topics.Include(c => c.Course).Where(c => c.Name == edit.Topic.Name && c.CourseID == edit.Topic.CourseID);

				if (check.Count() > 0)
				{
					ModelState.AddModelError("Name", "Topic Already Exists.");
				}
				else
				{
					var toppicInDb = _context.Topics.Find(edit.Topic.ID);
					toppicInDb.Name = edit.Topic.Name;
					toppicInDb.Description = edit.Topic.Description;
					_context.SaveChanges();
					return RedirectToAction("Index");
				}
			}

			var topicVM = new CourseTopicViewModel()
			{
				Courses = _context.Courses.ToList(),
				Topic = edit.Topic,
			};
			return View(topicVM);
		}

		//GET: Delete
		[HttpGet]
		[Authorize(Roles = "Staff")]
		public ActionResult Delete(int id)
		{
			var topicInDb = _context.Topics.SingleOrDefault(t => t.ID == id);
			if (topicInDb == null)
			{
				return HttpNotFound();
			}
			_context.Topics.Remove(topicInDb);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}