﻿using System;
using System.Windows.Forms;
using System.Windows.Input;
using System.Collections.Generic;
using _4RTools.Utils;
using _4RTools.Model;
using System.Linq;
using System.Windows.Media.Effects;

namespace _4RTools.Forms
{
    public partial class AutoBuffStatusForm : Form, IObserver
    {
        private List<BuffContainer> debuffContainers = new List<BuffContainer>();

        public AutoBuffStatusForm(Subject subject)
        {
            InitializeComponent();
            debuffContainers.Add(new BuffContainer(this.DebuffsGP, Buff.GetDebuffs()));

            new DebuffRenderer(debuffContainers, toolTip1).doRender();

            this.txtStatusKey.KeyDown += new System.Windows.Forms.KeyEventHandler(FormUtils.OnKeyDown);
            this.txtStatusKey.KeyPress += new KeyPressEventHandler(FormUtils.OnKeyPress);
            this.txtStatusKey.TextChanged += new EventHandler(onStatusKeyChange);

            subject.Attach(this);
        }

        public void Update(ISubject subject)
        {
            switch ((subject as Subject).Message.code)
            {
                case MessageCode.PROFILE_CHANGED:
                    this.txtStatusKey.Text = ProfileSingleton.GetCurrent().StatusRecovery.buffMapping.Keys.Contains(EffectStatusIDs.SILENCE) ? ProfileSingleton.GetCurrent().StatusRecovery.buffMapping[EffectStatusIDs.SILENCE].ToString() : Keys.None.ToString();
                    doUpdate(this);
                    break;
                case MessageCode.TURN_OFF:
                    ProfileSingleton.GetCurrent().DebuffsRecovery.Stop();
                    ProfileSingleton.GetCurrent().StatusRecovery.Stop();
                    break;
                case MessageCode.TURN_ON:
                    ProfileSingleton.GetCurrent().DebuffsRecovery.Start();
                    ProfileSingleton.GetCurrent().StatusRecovery.Start();
                    break;
            }
        }
        public static void doUpdate(Control control)
        {
            var autobuffDict = ProfileSingleton.GetCurrent().DebuffsRecovery.buffMapping;
            var groupbox = control.Controls.OfType<GroupBox>().FirstOrDefault();
            foreach (TextBox txt in groupbox.Controls.OfType<TextBox>()) {
                var buffid = int.Parse(txt.Name.Split('n')[1]);
                var existe = autobuffDict.FirstOrDefault(x => x.Key.Equals((EffectStatusIDs)buffid));
                if (existe.Key != 0)
                {
                    txt.Text = autobuffDict[(EffectStatusIDs)buffid].ToString();
                }
                else
                {
                    txt.Text = "None";
                }
            }
        }
        private void onStatusKeyChange(object sender, EventArgs e)
        {
            Key k = (Key)Enum.Parse(typeof(Key), this.txtStatusKey.Text.ToString());

            ProfileSingleton.GetCurrent().StatusRecovery.AddKeyToBuff(EffectStatusIDs.POISON, k);
            ProfileSingleton.GetCurrent().StatusRecovery.AddKeyToBuff(EffectStatusIDs.SILENCE, k);
            ProfileSingleton.GetCurrent().StatusRecovery.AddKeyToBuff(EffectStatusIDs.BLIND, k);
            ProfileSingleton.GetCurrent().StatusRecovery.AddKeyToBuff(EffectStatusIDs.CONFUSION, k);
            ProfileSingleton.GetCurrent().StatusRecovery.AddKeyToBuff(EffectStatusIDs.HALLUCINATIONWALK, k);
            ProfileSingleton.GetCurrent().StatusRecovery.AddKeyToBuff(EffectStatusIDs.HALLUCINATION, k);
            ProfileSingleton.GetCurrent().StatusRecovery.AddKeyToBuff(EffectStatusIDs.CURSE, k);

            ProfileSingleton.SetConfiguration(ProfileSingleton.GetCurrent().StatusRecovery);
            this.ActiveControl = null;
        }

        private void AutoBuffStatusForm_Load(object sender, EventArgs e)
        {

        }
    }
}
