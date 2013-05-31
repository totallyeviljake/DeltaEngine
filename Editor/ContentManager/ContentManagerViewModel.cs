using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using DeltaEngine.Editor.Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.ContentManager
{
	/// <summary>
	/// passes the data used in the ContentManagerView
	/// </summary>
	public sealed class ContentManagerViewModel : ViewModelBase
	{
		public ContentManagerViewModel(ContentService content)
		{
			this.content = content;
			content.GetProjects();
			RaisePropertyChanged("Projects");
			SetMessenger();
			Images = new ObservableCollection<string>();
			ImageWidth = 357;
			ImageHeight = 361;
		}

		private void SetMessenger()
		{
			Messenger.Default.Register<string>(this, "DeletingImage", DeleteImageFromList);
			Messenger.Default.Register<IDataObject>(this, "AddImage", DropContent);
			Messenger.Default.Register<Size>(this, "ChangeImageSize", ChangeImageSize);
			Messenger.Default.Register<string>(this, "AddProject", AddNewProject);
			Messenger.Default.Register<List<string>>(this, "SaveImagesAsAnimation", SaveImagesAsAnimation);
		}

		private ContentService content;
		public ObservableCollection<string> Images { get; set; }

		public void DropContent(IDataObject dropObject)
		{
			if (!dropObject.GetDataPresent(DataFormats.FileDrop))
				return;

			var files = (string[])dropObject.GetData(DataFormats.FileDrop);
			foreach (var file in files)
				AddImage(file);
		}

		private void AddImage(string imageFilePath)
		{
			using (Stream stream = File.OpenRead(imageFilePath))
			{
				content.AddContent(SelectedProject, Path.GetFileName(imageFilePath), stream);
				Images = new ObservableCollection<string>(content.GetContentNames(SelectedProject));
			}
			RaisePropertyChanged("Images");
		}

		public string SelectedProject
		{
			get { return selectedProject; }
			set { ChangeSelectedProject(value); }
		}

		private string selectedProject;

		private void ChangeSelectedProject(string value)
		{
			selectedProject = value;
			if (selectedProject != null)
				Images = new ObservableCollection<string>(content.GetContentNames(selectedProject));
			RaisePropertyChanged("Images");
			ViewImage = null;
			RaisePropertyChanged("ViewImage");
			EditViewImage();
		}

		public BitmapImage ViewImage { get; private set; }

		public void EditViewImage()
		{
			if (selectedContent == null)
				return;	

			var image = new BitmapImage();
			using (var contentStream = content.LoadContent(SelectedProject, SelectedContent))
			{
				image.BeginInit();
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.StreamSource = contentStream;
				image.EndInit();
			}
			ViewImage = image;
			RaisePropertyChanged("ViewImage");
		}

		public string SelectedContent
		{
			get { return selectedContent; }
			set
			{
				selectedContent = value;
				EditViewImage();
			}
		}

		private string selectedContent;

		public ObservableCollection<string> Projects
		{
			get { return new ObservableCollection<string>(content.GetProjects()); }
		}

		public void DeleteImageFromList(string msg)
		{
			ViewImage = null;
			RaisePropertyChanged("ViewImage");
			string deleteContent = selectedContent;
			Images.Remove(selectedContent);
			content.DeleteContent(SelectedProject, deleteContent);
		}

		public void AddNewProject(string obj)
		{
			if (string.IsNullOrEmpty(NewProjectName))
			{
				MessageBox.Show("Please type in a project name", "Warning");
				return;
			}

			content.AddProject(NewProjectName);
			content.GetProjects();
			RaisePropertyChanged("Projects");
		}

		public string NewProjectName { get; set; }

		public void ChangeImageSize(Size size)
		{
			ImageWidth = size.Width;
			ImageHeight = size.Height;
			RaisePropertyChanged("ImageWidth");
			RaisePropertyChanged("ImageHeight");
		}

		public double ImageWidth { get; set; }
		public double ImageHeight { get; set; }

		public void SaveImagesAsAnimation(List<string> itemlist)
		{
			if (string.IsNullOrEmpty(AnimationName) || string.IsNullOrEmpty(selectedProject))
			itemlist.Sort();
			content.SaveImagesAsAnimation(itemlist, AnimationName, selectedProject);
		}

		public string AnimationName { get; set; }
	}
}