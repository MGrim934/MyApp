using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AmazingMoodMonitor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        List <Mood> _moodList;
        List<Activity> _activityList;
        ArrayList _previousEntries = new ArrayList();
        Random roll = new Random();
        ArrayList moodDupCheck = new ArrayList();
        ArrayList actDupCheck = new ArrayList();
        Boolean bookOpen = false;

        string mood, activity;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
           
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //if the data is already loaded then exit the method
            if (_moodList != null && _activityList != null)
            {
                generateMoodGrid();
                generateActivityGrid();
                return;
            }

            _moodList = new List<Mood>();
            _activityList = new List<Activity>();
            loadLocalData();
            lvJournal.ItemsSource = _previousEntries;




            // lvMoods.ItemsSource = _moodList;


        }



        private async void loadLocalData()
        {
            var moodsFile = await Package.Current.InstalledLocation.GetFileAsync("Data\\moods.txt");
            var fileText = await FileIO.ReadTextAsync(moodsFile);


            try
            {
                var moodsJArray = JsonArray.Parse(fileText);
                createListofMoods(moodsJArray);
               
            }
            catch (Exception exJA)
            {
                MessageDialog dialog = new MessageDialog(exJA.Message);
                await dialog.ShowAsync();

            }


            var activitiesFile = await Package.Current.InstalledLocation.GetFileAsync("Data\\activities.txt");
            fileText = await FileIO.ReadTextAsync(activitiesFile);


            try
            {
                var activitiesJArray = JsonArray.Parse(fileText);
                createListofActivities(activitiesJArray);

            }
            catch (Exception exJA)
            {
                MessageDialog dialog = new MessageDialog(exJA.Message);
                await dialog.ShowAsync();

            }

            generateMoodGrid();
            generateActivityGrid();


        }//load data

        private void createListofMoods(JsonArray moodsJArray)
        {
            foreach (var item in moodsJArray)
            {
                // get the object
                var obj = item.GetObject();

               
                Mood mood = new Mood();

                // get each key value pair and sort it to the appropriate elements
                // of the class
                foreach (var key in obj.Keys)
                {
                    IJsonValue value;
                    if (!obj.TryGetValue(key, out value))
                        continue;

                    switch (key)
                    {
                        case "mood":
                            mood.name = value.GetString();
                            break;
                        case "image":
                           mood.image = value.GetString();
                            break;
                       
                    }
                } // end foreach (var key in obj.Keys)
                _moodList.Add(mood);
            } // end foreach (var item in array)
        }//createList of Moods



        private void createListofActivities(JsonArray actJArray)
        {
            foreach (var item in actJArray)
            {
                // get the object
                var obj = item.GetObject();


                Activity activity = new Activity();

                // get each key value pair and sort it to the appropriate elements
                // of the class
                foreach (var key in obj.Keys)
                {
                    IJsonValue value;
                    if (!obj.TryGetValue(key, out value))
                        continue;

                    switch (key)
                    {
                        case "activity":
                            activity.name = value.GetString();
                            break;
                        case "image":
                           activity.image = value.GetString();
                            break;

                    }
                } // end foreach (var key in obj.Keys)
                _activityList.Add(activity);
            } // end foreach (var item in array)
        }//createList of Moods


        private  void generateMoodGrid()
        {
            Image img;
            int moodChoice;
            //fill up grid
            for(int i = 0; i < 5; i++)
            {
                for(int j = 0; j < 2; j++)
                {
                    img = new Image();

                    do
                    {
                       moodChoice = roll.Next(0, (_moodList.Count));
                    } while (moodDupCheck.Contains(moodChoice));


                    img.Name = _moodList[moodChoice].name;
                    moodDupCheck.Add(moodChoice);
                    string thing = "ms-appx://"+ _moodList[moodChoice].image;

                    Uri pic = new Uri(thing);
                    ImageBrush fill = new ImageBrush();
                   fill.ImageSource = new BitmapImage(pic);
                    img.Source = fill.ImageSource;
                    img.Width = 75;
                    img.Height = 75;
                    

                    img.SetValue(Grid.RowProperty, i);
                    img.SetValue(Grid.ColumnProperty, j);
                    img.Tapped += Mood_Tapped;
                    gdMood.Children.Add(img);

                }
            }
        }

        private void Mood_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image curr = (Image)sender;
            //change text
            //show other grid
            gdMood.Visibility = Visibility.Collapsed;

            gdActivity.Visibility = Visibility.Visible;
            //get mood type
            //get activity type
            //get date
            //write to file
            JournalEntry entry = new JournalEntry();
            mood = curr.Name;

            



        }



        private  void generateActivityGrid()
        {
            Image img;
            int activityChoice;
            //fill up grid
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    img = new Image();

                    do
                    {
                       activityChoice = roll.Next(0, (_activityList.Count));
                    } while (actDupCheck.Contains(activityChoice));


                    img.Name = _activityList[activityChoice].name;
                   actDupCheck.Add(activityChoice);
                    string thing = "ms-appx://" + _activityList[activityChoice].image;

                    Uri pic = new Uri(thing);
                    ImageBrush fill = new ImageBrush();
                    fill.ImageSource = new BitmapImage(pic);
                    img.Source = fill.ImageSource;
                    img.Width = 100;
                    img.Height = 75;


                    img.SetValue(Grid.RowProperty, i);
                    img.SetValue(Grid.ColumnProperty, j);
                    img.Tapped +=Activity_Tapped;
                    gdActivity.Children.Add(img);

                }
            }
        }//activitygrid

        private void Activity_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gdMood.Visibility = Visibility.Visible;
            gdActivity.Visibility = Visibility.Collapsed;

            Image curr = (Image)sender;
            activity = curr.Name;


            createJournalEntry();
        }



        private void createJournalEntry()
        {
            JournalEntry entry = new JournalEntry();
            int index = _moodList.FindIndex(item => item.name.Equals(mood));
           
            entry.mood =_moodList[index].image;

            index = _activityList.FindIndex(item => item.name.Equals(activity));

            entry.activity = _activityList[index].image;
            
          
            entry.dateTime = DateTime.Now;
            //add to list
            _previousEntries.Add(entry);
            updateJournal();
            ptMain.SelectedIndex = 2; ;
           


        }

        private void tbNewEntry_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //change selected index
            ptMain.SelectedIndex = 1;
            //opens up journal
            changeBackground();
           
            


        }

        private void tbPrevious_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ptMain.SelectedIndex = 2;


            changeBackground();
           
        }

        private void changeBackground()
        {
            Uri pic = new Uri("ms-appx:///Images/bg.jpg",UriKind.RelativeOrAbsolute);
            ImageBrush fill = new ImageBrush();
            fill.ImageSource = new BitmapImage(pic);

            

            gdMaster.Background = fill;
            bookOpen = true;
        }

        private void closeBook(object sender, TappedRoutedEventArgs e)
        {
            Pivot curr = (Pivot)sender;

            if (curr.SelectedIndex == 0) { 
            Uri pic = new Uri("ms-appx:///Images/main.png", UriKind.RelativeOrAbsolute);
            ImageBrush fill = new ImageBrush();
            fill.ImageSource = new BitmapImage(pic);
                bookOpen = false;
                

            gdMaster.Background = fill;
            }
            else if(bookOpen!=true)
            {
                changeBackground();
                
            }
        
        }

        private void updateJournal()
        {
  
            lvJournal.ItemsSource = _previousEntries;
            lvJournal.ItemsSource = null;
            lvJournal.ItemsSource = _previousEntries;

            //open file or create file if there is none
            //check  mood
            //if there then give it the location of the image
            //activity check give image
            //read in date and time
            //update previous entries









        }
    }
}
