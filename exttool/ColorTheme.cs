using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BirdsEye {
    public static class ColorTheme {
        public static Color FloralWhite = Color.FromArgb(247, 244, 234);
        public static Color Lavender = Color.FromArgb(222, 217, 226);
        public static Color VistaBlue = Color.FromArgb(128, 161, 212);
        public static Color BurntSienna = Color.FromArgb(232, 110, 80);

        /// <summary>
        /// Apply the set color theme to every control in `control`.<br/>
        /// Postcondition: This method will mutate the passed control and it's 
        /// child controls (recursive).
        /// </summary>
        public static void ApplyColorTheme(Control control) {
            control.BackColor = Lavender;

            var groupboxes = Utils.GetAllControls(control, typeof(GroupBoxEx)).Cast<GroupBoxEx>();
            var labels = Utils.GetAllControls(control, typeof(Label));
            var buttons = Utils.GetAllControls(control, typeof(Button));
            var listboxes = Utils.GetAllControls(control, typeof(ListBox));
            var textboxes = Utils.GetAllControls(control, typeof(TextBox));
            foreach (GroupBoxEx ctrl in groupboxes) {
                ctrl.BorderColor = FloralWhite;
                ctrl.TextColor = VistaBlue;
            }
            foreach (Control ctrl in labels) {
                ctrl.ForeColor = VistaBlue;
            }
            foreach (Control ctrl in buttons) {
                ctrl.BackColor = VistaBlue;
                ctrl.ForeColor = FloralWhite;
            }
            foreach (Control ctrl in listboxes) {
                ctrl.BackColor = FloralWhite;
                ctrl.ForeColor = BurntSienna;
            }
            foreach (Control ctrl in textboxes) {
                ctrl.BackColor = FloralWhite;
                ctrl.ForeColor = VistaBlue;
            }
        }
    }
}