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

        static public string person_cmp(string dir) {
            string output = "";
            char[] delim = { '/', '\\' };
            char[] delim_ = { '_', ',' };
            List<Person> persons = new List<Person>();
            for (int i = 1; i < 101; ++i) {
                Person p = new Person();
                p.Id = i;
                persons.Add(p);
            }
            foreach (string file in Directory.GetFiles(dir, "*")) {
                string image = file.Split(delim)[file.Split(delim).Length - 1];
                int index = -1;
                int.TryParse(image.Split(delim_)[0], out index);
                Fingerprint fp = new Fingerprint();
                fp.AsBitmapSource = new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute));
                persons[index - 1].Fingerprints.Add(fp);
            }
            foreach (Person p in persons) { engine.Extract(p); }
            foreach (Person p in persons) {
                foreach (Person c in persons) {
                    float score = engine.Verify(p, c);
                    output += p.Id.ToString() + "," + c.Id.ToString() + "," + score + "\n";
                }
            }
            return output;
        }

        static public string dir_cmp(string dir_1, string dir_2) {
            string output = "";
            foreach (string file in Directory.GetFiles(dir_1, "*")) {
                Console.WriteLine(file);
                Fingerprint fp = new Fingerprint();
                fp.AsBitmapSource = new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute));
                Person person = new Person();
                person.Fingerprints.Add(fp);
                engine.Extract(person);
                foreach (string image in Directory.GetFiles(dir_2, "*")) {
                    if (file != image) {
                        Fingerprint cfp = new Fingerprint();
                        cfp.AsBitmapSource = new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute));
                        Person comparison = new Person();
                        comparison.Fingerprints.Add(cfp);
                        engine.Extract(comparison);
                        float score = engine.Verify(person, comparison);
                        bool match = (score > 0);
                        if (match) { Console.WriteLine(file + " ~ " + image + " Score: " + score); }
                        output += file + "," + image + "," + score + "\n";
                    }
                }
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

            //var t1 = new Thread(() => { full_full = dir_cmp(DB1_a_fullprint, DB1_a_fullprint); });
            //var t2 = new Thread(() => { part_part = dir_cmp(Db1_a_partial_dataset, Db1_a_partial_dataset); });
            //var t3 = new Thread(() => { full_part = dir_cmp(DB1_a_fullprint, Db1_a_partial_dataset); });
            var t4 = new Thread(() => { full_person = person_cmp(DB1_a_fullprint); });
            var t5 = new Thread(() => { part_person = person_cmp(Db1_a_partial_dataset); });

            //t1.Start();
            //t2.Start();
            //t3.Start();
            t4.Start();
            t5.Start();

            //t1.Join();
            //t2.Join();
            //t3.Join();
            t4.Join();
            t5.Join();

            //write(ref full_full, "full_full.csv");
            //write(ref part_part, "part_part.csv");
            //write(ref full_part, "full_part.csv");
            write(ref full_person, "full_person.csv");
            write(ref part_person, "part_person.csv");






            Console.WriteLine("DONE");
        }
    }
}
