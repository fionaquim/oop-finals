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
        private string filePath = "\"C:\\Users\\ZachyBoi\\source\\repos\\LibraryFiona\\LibraryFiona\\default_books.txt\"";
        private List<Book> books = new List<Book>();
        private int currentIndex = -1;

        public MainWindow()
        {
            InitializeComponent();

            // Define the default file path
            filePath = @"C:\Users\ZachyBoi\source\repos\LibraryFiona\LibraryFiona\default_books.txt"; // Update with your actual file path

            // Check if the file exists
            if (File.Exists(filePath))
            {
                LoadBookData(); // Load the file automatically
            }
            else
            {
                MessageBox.Show($"File not found: {filePath}. Please place the file in the correct location.", "File Not Found");
            }
        }


        private void LoadBookData()
        {
            try
            {
                books.Clear();

                // Use StreamReader to read the file line by line
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
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
                }

                if (books.Count > 0)
                {
                    PopulateComboBoxes(); // Populate filters
                    currentIndex = 0;     // Start with the first book
                    PopulateForm(books[currentIndex]); // Display the first book
                }
                else
                {
                    MessageBox.Show("No books found in the file.", "Info");
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "Error");
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
            // Get distinct authors and genres
            var uniqueAuthors = books.Select(b => b.Author).Distinct().OrderBy(a => a).ToList();
            var uniqueGenres = books.Select(b => b.Genre).Distinct().OrderBy(g => g).ToList();

            // Assign the updated lists to the ComboBoxes
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

            // Check if the book already exists by ISBN
            if (books.Any(b => b.ISBN == tbISBN.Text))
            {
                MessageBox.Show("A book with this ISBN already exists.", "Duplicate Entry");
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

            // Add the new book to the in-memory list and save
            books.Add(newBook);
            SaveBooksToFile();
            LoadBookData();
            MessageBox.Show("Book added successfully!");
            ClearForm(); // Clear the input form after adding
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

        private void BTN_UPDATE_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BTN_DELETE_Click(object sender, RoutedEventArgs e)
        {
            // Ensure a book is selected
            if (string.IsNullOrWhiteSpace(tbISBN.Text))
            {
                MessageBox.Show("No book selected to delete.", "Delete Error");
                return;
            }

            // Find the book to delete using the ISBN (unique identifier)
            var bookToDelete = books.FirstOrDefault(b => b.ISBN == tbISBN.Text);

            if (bookToDelete != null)
            {
                // Confirm deletion
                var result = MessageBox.Show($"Are you sure you want to delete the book \"{bookToDelete.Title}\"?", "Confirm Deletion", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    books.Remove(bookToDelete); // Remove the book from the list
                    SaveBooksToFile();         // Save the updated list to the file
                    LoadBookData();            // Reload the updated data
                    PopulateComboBoxes();      // Update the ComboBoxes
                    ClearForm();               // Clear the form
                    currentIndex = -1;         // Reset the index
                    MessageBox.Show("Book deleted successfully!");
                }
            }
            else
            {
                MessageBox.Show("Book not found in the list. Please try again.", "Error");
            }
        }

        private void BTN_CLEAR_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }
    }
}
