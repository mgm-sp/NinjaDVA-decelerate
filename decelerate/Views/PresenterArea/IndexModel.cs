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
        public string Color
        {
            get
            {
                var abs = Math.Abs(AverageSpeedChoice);
                if (abs > 50)
                {
                    return "danger";
                }
                else if (abs > 20)
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
                if (AverageSpeedChoice < -20)
                {
                    return "Slower";
                }
                else if (AverageSpeedChoice > 20)
                {
                    return "Faster";
                }
                else
                {
                    return "Perfect";
                }
            }
        }
    }
}
