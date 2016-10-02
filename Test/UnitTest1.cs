using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using View.RazorTemplate;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //poszlo w E:\Programowanie\EmailSender\Test\bin\Debug\View
            //bylo w stringu View\PersonMail.cshtml

            //E:\Programowanie\EmailSender\Test\bin\View
            //bylo w @"..\View\PersonMail.cshtml";


            var path = @"..\..\..\View\PersonMail.cshtml";
            FileInfo fileinfo = new FileInfo(path);
            Assert.AreEqual(fileinfo.Exists,true);
        }
    }
}
