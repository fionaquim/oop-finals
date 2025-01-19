using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // List of unique authors
            List<string> authors = new List<string>
            {
                "Harper Lee",
                "George Orwell",
                "F. Scott Fitzgerald",
                "Jane Austen",
                "J.K. Rowling",
                "J.D. Salinger",
                "J.R.R. Tolkien",
                "Ray Bradbury",
                "Herman Melville"
            };

            // Populate the Author ComboBox with unique items dynamically
            foreach (string author in authors)
            {
                Cb1.Items.Add(author);
            }

            // List of unique genres
            List<string> genres = new List<string>
            {
                "Fiction",
                "Dystopian",
                "Classic",
                "Romance",
                "Fantasy",
                "Adventure"
            };

            // Populate the Genre ComboBox with unique items dynamically
            foreach (string genre in genres)
            {
                Cb2.Items.Add(genre);
            }
        }

    }
}
