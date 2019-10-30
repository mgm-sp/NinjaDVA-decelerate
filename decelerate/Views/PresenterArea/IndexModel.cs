using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using decelerate.Models;

namespace decelerate.Views.PresenterArea
{
    public class IndexModel
    {
        public IEnumerable<User> Users { get; set; }
        public int AverageSpeedChoice
        {
            get
            {
                /* Calculate sum and count over all users who voted: */
                int sumSpeedChoice = 0;
                uint cntSpeedChoice = 0;
                foreach (var user in Users)
                {
                    if (user.SpeedChoice != null)
                    {
                        sumSpeedChoice += user.SpeedChoice ?? 0;
                        cntSpeedChoice++;
                    }
                }
                /* Check if users voted, if not return zero: */
                if (cntSpeedChoice == 0)
                {
                    return 0;
                }
                else
                {
                    return (int)((float)sumSpeedChoice / cntSpeedChoice);
                }
            }
        }
        public uint TransformedAverageSpeedChoice
        {
            get
            {
                return (uint)((100 + AverageSpeedChoice) / 2.0);
            }
        }
        public uint WarningThreshold
        {
            get
            {
                return 25;
            }
        }
        public uint DangerThreshold
        {
            get
            {
                return 75;
            }
        }
        public string Color
        {
            get
            {
                var abs = Math.Abs(AverageSpeedChoice);
                if (abs > DangerThreshold)
                {
                    return "danger";
                }
                else if (abs > WarningThreshold)
                {
                    return "warning";
                }
                else
                {
                    return "success";
                }
            }
        }
        public string Text
        {
            get
            {
                if (AverageSpeedChoice < -DangerThreshold)
                {
                    return "talk much slower";
                }
                else if (AverageSpeedChoice < -WarningThreshold)
                {
                    return "talk a bit slower";
                }
                else if (AverageSpeedChoice > DangerThreshold)
                {
                    return "talk much faster";
                }
                else if (AverageSpeedChoice > WarningThreshold)
                {
                    return "talk a bit faster";
                }
                else
                {
                    return "keep your speed";
                }
            }
        }
    }
}
