using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BirdsEye {
    public class Utils {
        /// <summary>
        /// Recursively find and return every nested control within `control`
        /// that is of type `type`.
        /// </summary>
        public static IEnumerable<Control> GetAllControls(Control control, Type type) {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllControls(ctrl, type))
                .Concat(controls)
                .Where(c => c.GetType() == type);
        }
    }
}