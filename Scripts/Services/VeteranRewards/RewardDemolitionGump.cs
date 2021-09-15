using System;
using Server.Items;
using Server.Multis;
using Server.Network;

namespace Server.Gumps
{ 
    public class RewardDemolitionGump : Gump
    {
        private readonly IAddon m_Addon;
        public RewardDemolitionGump(IAddon addon, int question)
            : base(150, 50)
        {
            this.m_Addon = addon;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddBackground(0, 0, 220, 170, 0x13BE);
            this.AddBackground(10, 10, 200, 150, 0xBB8);
			
            this.AddHtmlLocalized(20, 30, 180, 60, question, false, false); // Do you wish to re-deed this decoration?
			
            this.AddHtmlLocalized(55, 100, 150, 25, 1011011, false, false); // CONTINUE
            this.AddButton(20, 100, 0xFA5, 0xFA7, (int)Buttons.Confirm, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(55, 125, 150, 25, 1011012, false, false); // CANCEL
            this.AddButton(20, 125, 0xFA5, 0xFA7, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
        }

        private enum Buttons
        {
            Cancel,
            Confirm,
        }
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Item item = this.m_Addon as Item;
			
            if (item == null || item.Deleted)
                return;                

            if (info.ButtonID == (int)Buttons.Confirm)
            {
                Mobile m = sender.Mobile;
                BaseHouse house = BaseHouse.FindHouseAt(m);  
				
                if (house != null && house.IsOwner(m))
                {
                    if (m.InRange(item.Location, 2))
                    {
                        Item deed = this.m_Addon.Deed;
						
                        if (deed != null)
                        {
                            m.AddToBackpack(deed);           	
                            house.Addons.Remove(item);
                            item.Delete();      
                        }
                    }
                    else
                        m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                }
                else
                    m.SendLocalizedMessage(1049784); // You can only re-deed this decoration if you are the house owner or originally placed the decoration.
            }
        }
    }
}