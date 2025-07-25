using System.Windows;
using System.Windows.Controls;
using BE;

namespace Heyam
{    
    public partial class UC_Reminder : UserControl
    {
        public UC_Reminder()
        {
            InitializeComponent();
        }
        public Reminder ReminderData { get; set; }
        private void ShowDescriptionBTN_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ReminderData != null)
            {
                CustomMessageBox msgbox = new CustomMessageBox("توضیحات یادآور", ReminderData.Description, false, CustomMessageBox.MessageType.Info);
                msgbox.ShowDialog();
            }
        }
        private void RemoveReminderBTN_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var currentReminder = this;
            var parentGrid = currentReminder.Parent as Grid;

            if (parentGrid != null)
            {
                int deletedRow = Grid.GetRow(currentReminder);

                // اول حذف خود Reminder از Grid
                parentGrid.Children.Remove(currentReminder);

                // حالا بقیه یادآورها رو یک ردیف بالا می‌بریم
                foreach (UIElement child in parentGrid.Children)
                {
                    if (child is UC_Reminder reminder)
                    {
                        int currentRow = Grid.GetRow(reminder);
                        if (currentRow > deletedRow)
                        {
                            Grid.SetRow(reminder, currentRow - 1);
                        }
                    }
                }
            }
        }
    }
}