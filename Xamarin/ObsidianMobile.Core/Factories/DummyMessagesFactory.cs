using System;
using System.Collections.ObjectModel;
using ObsidianMobile.Core.Enums;
using ObsidianMobile.Core.Interfaces.Models;
using ObsidianMobile.Core.Models.Messages;
using ObsidianMobile.Core.Utils;

namespace ObsidianMobile.Core.Factories
{
    public static class DummyMessagesFactory
    {
        public static ObservableCollection<IMessage> GetMessages()
        {
            var messages = new ObservableCollection<IMessage>
            {
                new TextMessage(208145205, MessageType.Incoming, 1513687254, Server.CURRENT_USER_ID, DateTime.Now, "facebook.com \U0001F600 Lorem ipsum dolor #sit amet"),
                new TextMessage(208145205, MessageType.Outgoing,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "about.com \U0001F600 Lorem ipsum dolor @sit amet, #consetetur #sadipscing elitr, sed diam nonumy eirmod tempor "),
                new TextMessage(208145205, MessageType.Incoming,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor"),
                new TextMessage(208145205, MessageType.Outgoing,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "microsoft.com Lorem ipsum dolor sit amet, @consetetur sadipscing #elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est  \U0001F600"),
                new TextMessage(208145205, MessageType.Incoming,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "Es ist langweilig... \U0001F600"),
                new TextMessage(208145205, MessageType.Outgoing,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "\U0001F600 Lorem ipsum dolor sit @amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor "),
                new TextMessage(208145205, MessageType.Incoming,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor sit amet \U0001F600"),
                new TextMessage(208145205, MessageType.Outgoing,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "\U0001F600 google.com Lorem ipsum dolor sit amet"),
                new TextMessage(208145205, MessageType.Incoming,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "\U0001F600 #Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum."),
                new TextMessage(208145205, MessageType.Outgoing,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor sit amet"),
                new TextMessage(208145205, MessageType.Incoming,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor sit amet"),
                new TextMessage(208145205, MessageType.Outgoing,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "\U0001F600 Lorem ipsum dolor sit amet \U0001F600 \U0001F600 \U0001F600"),
                new TextMessage(208145205, MessageType.Incoming,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "facebook.com \U0001F600 Lorem ipsum dolor #sit amet"),
                new TextMessage(208145205, MessageType.Outgoing,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "about.com \U0001F600 Lorem ipsum dolor @sit amet, #consetetur #sadipscing elitr, sed diam nonumy eirmod tempor "),
                new TextMessage(208145205, MessageType.Incoming,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor"),
                new TextMessage(208145205, MessageType.Outgoing,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "facebook.com \U0001F600 Lorem ipsum dolor #sit amet"),
                new TextMessage(208145205, MessageType.Incoming,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "about.com \U0001F600 Lorem ipsum dolor @sit amet, #consetetur #sadipscing elitr, sed diam nonumy eirmod tempor "),
                new TextMessage(208145205, MessageType.Outgoing,1513687254, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor"),

                new TextMessage(699981449, MessageType.Incoming, 578309888, Server.CURRENT_USER_ID, DateTime.Now, "facebook.com \U0001F600 Lorem ipsum dolor #sit amet"),
                new TextMessage(699981449, MessageType.Outgoing,578309888, Server.CURRENT_USER_ID, DateTime.Now, "about.com \U0001F600 Lorem ipsum dolor @sit amet, #consetetur #sadipscing elitr, sed diam nonumy eirmod tempor "),
                new TextMessage(699981449, MessageType.Incoming,578309888, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor"),
                new TextMessage(699981449, MessageType.Outgoing,578309888, Server.CURRENT_USER_ID, DateTime.Now, "microsoft.com Lorem ipsum dolor sit amet, @consetetur sadipscing #elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est  \U0001F600"),
                new TextMessage(699981449, MessageType.Incoming,578309888, Server.CURRENT_USER_ID, DateTime.Now, "Es ist langweilig... \U0001F600"),
                new TextMessage(699981449, MessageType.Outgoing,578309888, Server.CURRENT_USER_ID, DateTime.Now, "\U0001F600 Lorem ipsum dolor sit @amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor "),
                new TextMessage(699981449, MessageType.Incoming,578309888, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor sit amet \U0001F600"),
                new TextMessage(699981449, MessageType.Outgoing,578309888, Server.CURRENT_USER_ID, DateTime.Now, "\U0001F600 google.com Lorem ipsum dolor sit amet"),
                new TextMessage(699981449, MessageType.Incoming,578309888, Server.CURRENT_USER_ID, DateTime.Now, "\U0001F600 #Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum."),
                new TextMessage(699981449, MessageType.Outgoing,578309888, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor sit amet"),
                new TextMessage(699981449, MessageType.Incoming,578309888, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor sit amet"),
                new TextMessage(699981449, MessageType.Outgoing,578309888, Server.CURRENT_USER_ID, DateTime.Now, "\U0001F600 Lorem ipsum dolor sit amet \U0001F600 \U0001F600 \U0001F600"),
                new TextMessage(699981449, MessageType.Incoming,578309888, Server.CURRENT_USER_ID, DateTime.Now, "facebook.com \U0001F600 Lorem ipsum dolor #sit amet"),
                new TextMessage(699981449, MessageType.Outgoing,578309888, Server.CURRENT_USER_ID, DateTime.Now, "about.com \U0001F600 Lorem ipsum dolor @sit amet, #consetetur #sadipscing elitr, sed diam nonumy eirmod tempor "),
                new TextMessage(699981449, MessageType.Incoming,578309888, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor"),
                new TextMessage(699981449, MessageType.Outgoing,578309888, Server.CURRENT_USER_ID, DateTime.Now, "facebook.com \U0001F600 Lorem ipsum dolor #sit amet"),
                new TextMessage(699981449, MessageType.Incoming,578309888, Server.CURRENT_USER_ID, DateTime.Now, "about.com \U0001F600 Lorem ipsum dolor @sit amet, #consetetur #sadipscing elitr, sed diam nonumy eirmod tempor "),
                new TextMessage(699981449, MessageType.Outgoing,578309888, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor"),

                new TextMessage(1270834840, MessageType.Incoming, 1086367793, Server.CURRENT_USER_ID, DateTime.Now, "facebook.com \U0001F600 Lorem ipsum dolor #sit amet"),
                new TextMessage(1270834840, MessageType.Outgoing,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "about.com \U0001F600 Lorem ipsum dolor @sit amet, #consetetur #sadipscing elitr, sed diam nonumy eirmod tempor "),
                new TextMessage(1270834840, MessageType.Incoming,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor"),
                new TextMessage(1270834840, MessageType.Outgoing,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "microsoft.com Lorem ipsum dolor sit amet, @consetetur sadipscing #elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est  \U0001F600"),
                new TextMessage(1270834840, MessageType.Incoming,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "Es ist langweilig... \U0001F600"),
                new TextMessage(1270834840, MessageType.Outgoing,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "\U0001F600 Lorem ipsum dolor sit @amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor "),
                new TextMessage(1270834840, MessageType.Incoming,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor sit amet \U0001F600"),
                new TextMessage(1270834840, MessageType.Outgoing,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "\U0001F600 google.com Lorem ipsum dolor sit amet"),
                new TextMessage(1270834840, MessageType.Incoming,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "\U0001F600 #Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum."),
                new TextMessage(1270834840, MessageType.Outgoing,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor sit amet"),
                new TextMessage(1270834840, MessageType.Incoming,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor sit amet"),
                new TextMessage(1270834840, MessageType.Outgoing,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "\U0001F600 Lorem ipsum dolor sit amet \U0001F600 \U0001F600 \U0001F600"),
                new TextMessage(1270834840, MessageType.Incoming,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "facebook.com \U0001F600 Lorem ipsum dolor #sit amet"),
                new TextMessage(1270834840, MessageType.Outgoing,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "about.com \U0001F600 Lorem ipsum dolor @sit amet, #consetetur #sadipscing elitr, sed diam nonumy eirmod tempor "),
                new TextMessage(1270834840, MessageType.Incoming,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor"),
                new TextMessage(1270834840, MessageType.Outgoing,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "facebook.com \U0001F600 Lorem ipsum dolor #sit amet"),
                new TextMessage(1270834840, MessageType.Incoming,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "about.com \U0001F600 Lorem ipsum dolor @sit amet, #consetetur #sadipscing elitr, sed diam nonumy eirmod tempor "),
                new TextMessage(1270834840, MessageType.Outgoing,1086367793, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor"),

                new TextMessage(561577645, MessageType.Incoming, 1787772043, Server.CURRENT_USER_ID, DateTime.Now, "facebook.com \U0001F600 Lorem ipsum dolor #sit amet"),
                new TextMessage(561577645, MessageType.Outgoing,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "about.com \U0001F600 Lorem ipsum dolor @sit amet, #consetetur #sadipscing elitr, sed diam nonumy eirmod tempor "),
                new TextMessage(561577645, MessageType.Incoming,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor"),
                new TextMessage(561577645, MessageType.Outgoing,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "microsoft.com Lorem ipsum dolor sit amet, @consetetur sadipscing #elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est  \U0001F600"),
                new TextMessage(561577645, MessageType.Incoming,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "Es ist langweilig... \U0001F600"),
                new TextMessage(561577645, MessageType.Outgoing,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "\U0001F600 Lorem ipsum dolor sit @amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor "),
                new TextMessage(561577645, MessageType.Incoming,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor sit amet \U0001F600"),
                new TextMessage(561577645, MessageType.Outgoing,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "\U0001F600 google.com Lorem ipsum dolor sit amet"),
                new TextMessage(561577645, MessageType.Incoming,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "\U0001F600 #Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum."),
                new TextMessage(561577645, MessageType.Outgoing,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor sit amet"),
                new TextMessage(561577645, MessageType.Incoming,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor sit amet"),
                new TextMessage(561577645, MessageType.Outgoing,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "\U0001F600 Lorem ipsum dolor sit amet \U0001F600 \U0001F600 \U0001F600"),
                new TextMessage(561577645, MessageType.Incoming,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "facebook.com \U0001F600 Lorem ipsum dolor #sit amet"),
                new TextMessage(561577645, MessageType.Outgoing,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "about.com \U0001F600 Lorem ipsum dolor @sit amet, #consetetur #sadipscing elitr, sed diam nonumy eirmod tempor "),
                new TextMessage(561577645, MessageType.Incoming,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor"),
                new TextMessage(561577645, MessageType.Outgoing,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "facebook.com \U0001F600 Lorem ipsum dolor #sit amet"),
                new TextMessage(561577645, MessageType.Incoming,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "about.com \U0001F600 Lorem ipsum dolor @sit amet, #consetetur #sadipscing elitr, sed diam nonumy eirmod tempor "),
                new TextMessage(561577645, MessageType.Outgoing,1787772043, Server.CURRENT_USER_ID, DateTime.Now, "Lorem ipsum dolor"),

            };

            return messages;
        }
    }
}
