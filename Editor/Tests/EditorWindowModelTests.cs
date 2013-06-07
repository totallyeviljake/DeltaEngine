using System;
using System.Windows;
using DeltaEngine.Editor.Common;
using DeltaEngine.Editor.Helpers;
using NUnit.Framework;

namespace DeltaEngine.Editor.Tests
{
	public class EditorWindowModelTests
	{
		//TODO
		public void LoginWithInvalidApiKeyShouldFail()
		{
			var model = CreateModelAndTryToLoginWithEmptyApiKey();
			model.OnApiKeyChanged.Execute("invalid");
			model.OnLoginButtonClicked.Execute(null);
			Assert.IsFalse(model.IsLoggedIn);
			//TODO: Assert.AreEqual("Please obtain your API Key and login.", model.Error);
		}

		private static EditorWindowModel CreateModelAndTryToLoginWithEmptyApiKey()
		{
			var model = new EditorWindowModel(CreateEditorPluginLoaderWithoutLoadingAnyPlugins());
			model.OnApiKeyChanged.Execute("");
			model.OnLoginButtonClicked.Execute(null);
			return model;
		}

		private static EditorPluginLoader CreateEditorPluginLoaderWithoutLoadingAnyPlugins()
		{
			return new EditorPluginLoader("..");
		}

		[Test]
		public void LoginPanelShouldBeVisibleWhenNotLoggedIn()
		{
			var model = CreateModelAndTryToLoginWithEmptyApiKey();
			if (model.IsLoggedIn)
			{
				Assert.AreEqual(Visibility.Hidden, model.LoginPanelVisibility);
				Assert.AreEqual(Visibility.Visible, model.EditorPanelVisibility);
			}
			else
			{
				Assert.AreEqual(Visibility.Visible, model.LoginPanelVisibility);
				Assert.AreEqual(Visibility.Hidden, model.EditorPanelVisibility);
			}
		}

		//ncrunch: no coverage start
		[Test, Category("Slow"), Ignore]
		public void LoginWithValidApiKeyShouldPass()
		{
			var model = CreateModelAndTryToLoginWithEmptyApiKey();
			Assert.IsFalse(model.IsLoggedIn);
			model.OnApiKeyChanged.Execute("123abc");
			model.OnLoginButtonClicked.Execute(null);
			Assert.IsTrue(model.IsLoggedIn);
		}

		
		[Test, Category("Slow")]
		public void LoadApiKeyFromRegistry()
		{
			var model = CreateModelAndTryToLoginWithEmptyApiKey();
			var rememberKey = model.ApiKey;
			Console.WriteLine("Current API Key from Registry: " + rememberKey);
			model.OnApiKeyChanged.Execute("123");
			model.SaveApiKey();
			Assert.AreEqual("123", model.ApiKey);
			model.OnApiKeyChanged.Execute(rememberKey);
			model.SaveApiKey();
		}

		[Test, Ignore]
		public void WebsiteWhereApiKeyCanBeObtainedShouldBeOpened()
		{
			var model = CreateModelAndTryToLoginWithEmptyApiKey();
			model.OnGetApiKeyClicked.Execute(null);
		}

		[Test, Category("Slow"), Ignore]
		public void CreateEditorPluginEntryFromLoadedPlugins()
		{
			var mockPlugins = GetEditorPluginLoaderMock();
			var model = new EditorWindowModel(mockPlugins);
			model.AddAllPlugins();
			Assert.AreEqual(1, model.EditorPlugins.Count);
			Assert.AreEqual("MockEditorPlugin", model.EditorPlugins[0].ShortName);
			Assert.AreEqual("Mock.png", model.EditorPlugins[0].Icon);
			Assert.AreEqual(EditorPluginCategory.Settings, model.EditorPlugins[0].Category);
			Assert.AreEqual(typeof(MockEditorPluginView), model.EditorPlugins[0].GetType());
		}

		private static EditorPluginLoader GetEditorPluginLoaderMock()
		{
			var mockPlugins = new EditorPluginLoader("..");
			mockPlugins.UserControlsType.Clear();
			mockPlugins.UserControlsType.Add(typeof(MockEditorPluginView));
			return mockPlugins;
		}
	}
}