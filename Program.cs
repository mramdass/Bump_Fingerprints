//
//  Munieshwar Ramdass
//  Bump Fingerprint
//  Fall 2016
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Media.Imaging;
using SourceAFIS.Simple;

namespace AFIS {
    class Program {
        static AfisEngine engine = new AfisEngine();

        static public void write(ref string output, string path = "output.csv") {
            using (StreamWriter w = new StreamWriter(path)) {
                w.WriteLine(output);
            }
        }

        // Compares all subjects' fingerprints to a single image at a time
        static public string cmp(string dir) {
            string output = "";
            char[] delim = { '/', '\\' };
            char[] delim_ = { '_', ',' };
            string[] files = Directory.GetFiles(dir, "*");
            List<Person> persons = new List<Person>();
            List<Fingerprint> fingers = new List<Fingerprint>();
            List<string> image_order = new List<string>();
            for (int i = 1; i < files.Length / 8 + 1; ++i) {
                Person p = new Person();
                p.Id = i;
                persons.Add(p);
            }
            Console.WriteLine("\tReading images...");
            foreach (string file in files) {
                string image = file.Split(delim)[file.Split(delim).Length - 1];
                int index = -1;
                int.TryParse(image.Split(delim_)[0], out index);
                int finger_position = 0;
                int.TryParse(image.Split(delim_)[1], out finger_position);
                Fingerprint fp = new Fingerprint();
                fp.Finger = (Finger)finger_position;
                image_order.Add(image);
                fp.AsBitmapSource = new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute));
                persons[index - 1].Fingerprints.Add(fp);
                fingers.Add(fp);
            }
            int counter = 0;
            Console.WriteLine("\tExtracting persons...");
            Parallel.ForEach(persons, (p) => { engine.Extract(p); Console.WriteLine(++counter); });
            counter = 0;
            int total = persons.Count * fingers.Count;
            Console.WriteLine("\tVerifing comparisons...");
            Parallel.For(0, fingers.Count, (f) => {
                Person c = new Person();
                c.Fingerprints.Add(fingers[f]);
                engine.Extract(c);
                foreach (Person p in persons) {
                    float score = engine.Verify(p, c);
                    output += p.Id.ToString() + "," + image_order[f] + "," + score + "\n";
                    Console.WriteLine(++counter + "\t/ " + total);
                }
            });
            return output;
        }

        // Compares all combinations of subjects' own fingerprints
        static public string self_cmp(string dir) {
            string output = "";
            char[] delim = { '/', '\\' };
            char[] delim_ = { '_', ',' };
            string[] files = Directory.GetFiles(dir, "*");
            List<Person> persons = new List<Person>();
            List<string> image_order = new List<string>();
            for (int i = 1; i < files.Length + 1; ++i) {
                Person p = new Person();
                persons.Add(p);
            }
            Console.WriteLine("\tReading images...");
            foreach (string file in files) {
                string image = file.Split(delim)[file.Split(delim).Length - 1];
                int index = -1;
                int.TryParse(image.Split(delim_)[0], out index);
                Fingerprint fp = new Fingerprint();
                image_order.Add(image);
                fp.AsBitmapSource = new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute));
                persons[index - 1].Fingerprints.Add(fp);
            }
            int counter = 0;
            Console.WriteLine("\tExtracting persons...");
            Parallel.ForEach(persons, (p) => { engine.Extract(p); Console.WriteLine(++counter); });
            counter = 0;
            Console.WriteLine("\tVerifing comparisons...");
            Parallel.For(0, persons.Count, (c) => {
                for (int p = 0; p < persons.Count; ++p) {
                    if (image_order[p].Split(delim_)[0] == image_order[c].Split(delim_)[0]) {
                        float score = engine.Verify(persons[p], persons[c]);
                        output += image_order[p] + "," + image_order[c] + "," + score + "\n";
                        Console.WriteLine(++counter);
                    }
                }
            });
            return output;
        }

        // Compares subjects' fingerprints subject by subject
        static public string person_cmp(string dir) {
            string output = "";
            char[] delim = { '/', '\\' };
            char[] delim_ = { '_', ',' };
            string[] files = Directory.GetFiles(dir, "*");
            List<Person> persons = new List<Person>();
            for (int i = 1; i < files.Length + 1; ++i) {
                Person p = new Person();
                p.Id = i;
                persons.Add(p);
            }
            Console.WriteLine("\tReading images...");
            foreach (string file in files) {
                string image = file.Split(delim)[file.Split(delim).Length - 1];
                int index = -1;
                int.TryParse(image.Split(delim_)[0], out index);
                Fingerprint fp = new Fingerprint();
                fp.AsBitmapSource = new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute));
                persons[index - 1].Fingerprints.Add(fp);
            }
            int counter = 0;
            Console.WriteLine("\tExtracting persons...");
            Parallel.ForEach(persons, (p) => { engine.Extract(p); Console.WriteLine(++counter); });
            counter = 0;
            int total = persons.Count * persons.Count;
            Console.WriteLine("\tVerifing comparisons...");
            foreach (Person p in persons) {
                Parallel.ForEach(persons, (c) => {
                    float score = engine.Verify(p, c);
                    output += p.Id.ToString() + "," + c.Id.ToString() + "," + score + "\n";
                    Console.WriteLine(++counter + "\t/ " + total);
                });
            }
            return output;
        }

        // Compares pairs of fingerprint images - Note the run time of this function is long
        static public string dir_cmp(string dir_1, string dir_2) {
            string output = "";
            foreach (string file in Directory.GetFiles(dir_1, "*")) {
                Parallel.ForEach(Directory.GetFiles(dir_2, "*"), (image) => {
                    Fingerprint fp = new Fingerprint();
                    fp.AsBitmapSource = new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute));
                    Person person = new Person();
                    person.Fingerprints.Add(fp);
                    engine.Extract(person);

                    Fingerprint cfp = new Fingerprint();
                    cfp.AsBitmapSource = new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute));
                    Person comparison = new Person();
                    comparison.Fingerprints.Add(cfp);
                    engine.Extract(comparison);

                    float score = engine.Verify(person, comparison);
                    bool match = (score > 0);
                    if (match) { Console.WriteLine(file + " ~ " + image + " Score: " + score); }
                    output += file + "," + image + "," + score + "\n";
                });
            }
            return output;
        }

        static void Main(string[] args) {
            
            string DB1_a_fullprint = "C:/Users/mramd/Documents/Fingerprinting/DB1_a_fullprint";
            string Db1_a_partial_dataset = "C:/Users/mramd/Documents/Fingerprinting/Db1_a_partial_dataset";

            string full_full = null;
            string part_part = null;
            string full_part = null;
            string full_person = null;
            string part_person = null;
            string many_one_full = null;
            string many_one_part = null;
            string self_full = null;
            string self_part = null;


            //var t1 = new Thread(() => { full_full = dir_cmp(DB1_a_fullprint, DB1_a_fullprint); });
            //var t2 = new Thread(() => { part_part = dir_cmp(Db1_a_partial_dataset, Db1_a_partial_dataset); });
            //var t3 = new Thread(() => { full_part = dir_cmp(DB1_a_fullprint, Db1_a_partial_dataset); });
            //var t4 = new Thread(() => { full_person = person_cmp(DB1_a_fullprint); });
            //var t5 = new Thread(() => { part_person = person_cmp(Db1_a_partial_dataset); });
            var t6 = new Thread(() => { many_one_full = cmp(DB1_a_fullprint); });
            //var t7 = new Thread(() => { many_one_part = cmp(Db1_a_partial_dataset); });
            var t8 = new Thread(() => { self_full = self_cmp(DB1_a_fullprint); });
            //var t9 = new Thread(() => { self_part = self_cmp(Db1_a_partial_dataset); });

            //t1.Start();
            //t2.Start();
            //t3.Start();
            //t4.Start();
            //t5.Start();
            t6.Start();
            //t7.Start();
            t8.Start();
            //t9.Start();

            //t1.Join();
            //t2.Join();
            //t3.Join();
            //t4.Join();
            //t5.Join();
            t6.Join();
            //t7.Join();
            t8.Join();
            //t9.Join();

            //write(ref full_full, "full_full.csv");
            //write(ref part_part, "part_part.csv");
            //write(ref full_part, "full_part.csv");
            //write(ref full_person, "full_person.csv");
            //write(ref part_person, "part_person.csv");
            write(ref many_one_full, "many_one_full.csv");
            //write(ref many_one_part, "many_one_part.csv");
            write(ref self_full, "self_full.csv");
            //write(ref self_part, "self_part.csv");

            Console.WriteLine("DONE");
        }
    }
}
