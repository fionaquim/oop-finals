using Microsoft.Win32;
using System.IO;
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


namespace LibraryFiona
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string filePath = "";
        private List<Book> books = new List<Book>();
        private int currentIndex = -1;

        public MainWindow()
        {
            InitializeComponent();


        }

        private void BTN_Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"; //restrict to text files when choosing input file

            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName; // store the selected file path
                LoadBookData(); // load data from the file
            }

        }

        private void LoadBookData()
        {
            try
            {
                books.Clear();
                var lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 5)
                    {
                        books.Add(new Book
                        {
                            Title = parts[0],
                            Author = parts[1],
                            PublishedDate = parts[2],
                            ISBN = parts[3],
                            Genre = parts[4]
                        });
                    }
                }

                if (books.Count > 0)
                {
                    PopulateComboBoxes(); // Populate Author and Genre combo boxes
                    currentIndex = 0; // Start with the first book
                    PopulateForm(books[currentIndex]); // Populate the form with the first book's data
                }
                else
                {
                    MessageBox.Show("No books found in the file.", "Info");
                    ClearForm();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading File: " + ex.Message);
            }

        }

        private void PopulateForm(Book book)
        {
            if (book != null)
            {
                tbTITLE.Text = book.Title;
                Cb_Author.Text = book.Author;
                tbPD.Text = book.PublishedDate;
                tbISBN.Text = book.ISBN;
                Cb_Genre.Text = book.Genre;
            }
        }

        private void PopulateComboBoxes()
        {
            Cb_Author.Items.Clear();
            Cb_Genre.Items.Clear();

            var uniqueAuthors = books.Select(b => b.Author).Distinct().OrderBy(a => a).ToList();
            var uniqueGenres = books.Select(b => b.Genre).Distinct().OrderBy(g => g).ToList();

            Cb_Author.ItemsSource = uniqueAuthors;
            Cb_Genre.ItemsSource = uniqueGenres;

        }

        private void ClearForm()
        {
            tbTITLE.Text = "";
            Cb_Author.SelectedIndex = -1;
            tbPD.Text = "";
            tbISBN.Text = "";
            Cb_Genre.SelectedIndex = -1;

        }

        public class Book
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Author { get; set; }
            public string PublishedDate { get; set; }
            public string ISBN { get; set; }
            public string Genre { get; set; }
        }

        private void BTN_CLEAR_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void Cb_Author_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedAuthor = Cb_Author.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedAuthor))
            {
                var filteredBooks = books.Where(b => b.Author == selectedAuthor).ToList();
                DisplayFilteredBooks(filteredBooks);
            }
        }

        private void Cb_Genre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedGenre = Cb_Genre.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedGenre))
            {
                var filteredBooks = books.Where(b => b.Genre == selectedGenre).ToList();
                DisplayFilteredBooks(filteredBooks);
            }
        }

        // Display the first book from the filtered list or show a message if none match
        private void DisplayFilteredBooks(List<Book> filteredBooks)
        {
            if (filteredBooks.Any())
            {
                PopulateForm(filteredBooks.First());
            }
            else
            {
                MessageBox.Show("No books found for the selected criteria.");
                ClearForm();
            }
        }

        private void BTN_ADD_Click(object sender, RoutedEventArgs e)
        {
            // Ensure all fields are filled
            if (string.IsNullOrWhiteSpace(tbTITLE.Text) || string.IsNullOrWhiteSpace(Cb_Author.Text) ||
                string.IsNullOrWhiteSpace(tbPD.Text) || string.IsNullOrWhiteSpace(tbISBN.Text) ||
                string.IsNullOrWhiteSpace(Cb_Genre.Text))
            {
                MessageBox.Show("All fields must be filled out!", "Validation Error");
                return;
            }

            // Create a new book object from the input fields
            var newBook = new Book
            {
                Title = tbTITLE.Text,
                Author = Cb_Author.Text,
                PublishedDate = tbPD.Text,
                ISBN = tbISBN.Text,
                Genre = Cb_Genre.Text
            };

            books.Add(newBook); // Add the new book to the in-memory list
            SaveBooksToFile();  // Save updated data back to the file
            LoadBookData();     // Reload the updated data from the file
            MessageBox.Show("Book added successfully!");
        }

        private void SaveBooksToFile()
        {
            try
            {
                // Convert each book object to a line of text using '|' as a delimiter
                var lines = books.Select(b => $"{b.Title}|{b.Author}|{b.PublishedDate}|{b.ISBN}|{b.Genre}");
                File.WriteAllLines(filePath, lines); // Overwrite the file with updated data
            }
            catch (Exception ex)
            {
                // Display an error message if saving fails
                MessageBox.Show("Error saving file: " + ex.Message, "Error");
            }
        }
    }
}
