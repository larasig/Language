using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ditw.Test.Lang.RegexTokenUnitTest
{
    [TestClass]
    public class FullStoryTest_TeamGhostshell
    {
        const String _text = @"As part of what it’s calling “Project Blackstar,” the hacking collective Team Ghostshell posted approximately 2.5 million records it claims belong to Russian individuals who work across the political, educational and law enforcement spectrum online earlier this morning. With the project, detailed in a post on Pastebin, the group claims it is “declaring war on Russia’s cyberspace” to call attention to the country’s “tyranny and regret.” In a series of text documents posted to Github, the group spills a slew of information it claims belongs to “governmental, educational, academical, political, law enforcement, telecom, research institutes, medical facilities, large corporations (both national and international branches) in such fields as energy, petroleum, banks, dealerships and many more.” The rest of the Pastebin post goes on to call out the Russian government and is littered with references to corruption, capitalism and social injustice. Judging by some of the records released, the Russian Police, along with Novatek, Russia's largest independent natural gas producer, the Alfa Group, an investment consortium and JINR, the country’s Joint Institute for Nuclear Research, all appear to have been implicated in the alleged leak. Some records appear to include individuals’ usernames and passwords while other documents almost read like resumes, complete with individuals’ names, IP addresses, education and job history. Team Ghostshell released thousands of records last month too when the group announced it had published 120,000 records from some of the world’s top universities. That leak, dubbed “Project WestWind,” sought to “raise awareness towards the changes made in today’s education,“ spilling student and faculty email addresses, passwords and IDs. In “Project WestWind,” the group’s leader DeadMellox claimed the group could’ve posted hundreds of thousands more records than it posted. Similarly, today DeadMellox bragged the group “has access to more Russian files than the FSB (Federal Security Service)” a reference to the Russian intelligence agency. Commenting on this Article is closed.";
        [TestMethod]
        public void TeamGhostshell_Test1()
        {
            var token = Loader.GetToken(
                @"prod\prod_test.xml",
                null,
                new String[] { @"prod\names.reggrp", @"prod\pos.reggrp" }
                );

            Loader.RunTest_Match(token, _text, new String[] { } );

        }
    }
}
