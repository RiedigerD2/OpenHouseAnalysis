using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPF_Log_Analysis
{
    class Node: IComparable<Node>
    {
        List<Node> children;
        public string title;
        public int count;
        public Node(string title)
        {
            this.title = title;
            children = new List<Node>();
            count = 0;
        }
      public void  Add(List<string> list){

            foreach(Node n in children){
                if (n == null) { continue; }
                if (n.title.Equals(list[0]))
                {
                    if (list.Count() <=1)
                    {
                        n.count++;
                        return;
                    }
                    n.Add(list.Skip(1).ToList());
                    return;
                }
            }
          //list[0] is not in children
            Node newNode = new Node(list[0]);
            
            if (list.Count<=1)
            {
                
                newNode.count++;
                children.Add(newNode);
                return;
            }
            else
            {
                
                newNode.Add(list.Skip(1).ToList());
                newNode.count++;
                children.Add(newNode);
                return;
            }

        }


      public int CompareTo(Node other)
      {
          return other.count - this.count;
      }

      public void print(int tabs,System.IO.StreamWriter fh)
      {
          children.Sort();
          foreach (Node n in children)
          {
              for (int i = 0; i < tabs; i++)
              {
                  fh.Write("\t");
              }
              fh.WriteLine(n.title+" "+n.count);
              n.print(tabs + 1,fh);

          }
      }
      public string GetInfo(int tabs)
      {
          children.Sort();
          string info = "";
          foreach (Node n in children)
          {
              for (int i = 0; i < tabs; i++)
              {
                  info += "\t";
              }
              
              info += n.title + " " + n.count +"\n";
              
              info+=n.GetInfo(tabs + 1);
          }
          return info;
      }
        
    }
}
