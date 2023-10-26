using System.Windows.Forms;
using System.Drawing;

public static class PlaceholderManager
{
    public static void AddPlaceholder(this TextBox tb, string placeholderText)
    {
        bool hasSystemPasswordChar = (bool)typeof(TextBox).GetProperty("UseSystemPasswordChar").GetValue(tb, null);
        if (hasSystemPasswordChar)
        {
            tb.UseSystemPasswordChar = false;
        }

        tb.ForeColor = Color.DimGray;
        tb.Text = placeholderText;
        tb.Font = new Font(tb.Font, FontStyle.Regular);

        tb.Enter += (sender, e) =>
        {
            if (tb.Text == placeholderText)
            {
                tb.Text = "";
                tb.ForeColor = Color.Black;
                tb.Font = new Font(tb.Font, FontStyle.Regular);

                if (hasSystemPasswordChar)
                {
                    tb.UseSystemPasswordChar = true;
                }
            }
        };
        tb.Leave += (sender, e) =>
        {
            if (tb.Text == "")
            {
                tb.ForeColor = Color.DimGray;
                tb.Text = placeholderText;
                tb.Font = new Font(tb.Font, FontStyle.Regular);

                if (hasSystemPasswordChar)
                {
                    tb.UseSystemPasswordChar = false;
                }
            }
        };
    }
}

