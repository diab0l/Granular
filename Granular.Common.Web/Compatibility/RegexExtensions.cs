using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace System.Text.RegularExpressions
{
    public class Capture
    {
        public int Index { get; private set; }
        public string Value { get; private set; }
        public int Length { get { return Value.Length; } }

        public Capture(int index, string value)
        {
            this.Index = index;
            this.Value = value.DefaultIfNullOrEmpty();
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public class Group : Capture
    {
        public bool Success { get; private set; }

        public Group(int index, string value, bool success) :
            base(index, value)
        {
            this.Success = success;
        }
    }

    public class Match : Group
    {
        public GroupCollection Groups { get; private set; }

        private RegexMatch regexMatch;

        public Match(RegexMatch regexMatch) :
            base(regexMatch != null ? regexMatch.Index : 0, regexMatch != null ? regexMatch.Input : String.Empty, regexMatch != null)
        {
            this.regexMatch = regexMatch;

            if (regexMatch == null)
            {
                Groups = GroupCollection.Empty;
            }
            else
            {
                int startIndex = regexMatch.Index;
                Group[] groups = new Group[regexMatch.Length];
                for (int i = 0; i < regexMatch.Length; i++)
                {
                    groups[i] = new Group(regexMatch.Input.IndexOf(regexMatch[i], startIndex), regexMatch[i], !String.IsNullOrEmpty(regexMatch[i]));
                    startIndex = groups[i].Index;
                }

                Groups = new GroupCollection(groups);
            }
        }
    }

    public class GroupCollection : IEnumerable<Group>
    {
        public static readonly GroupCollection Empty = new GroupCollection(new Group[0]);

        public int Count { get { return groups.Count(); } }
        public Group this[int groupnum] { get { return groups.ElementAt(groupnum); } }

        private IEnumerable<Group> groups;

        public GroupCollection(IEnumerable<Group> groups)
        {
            this.groups = groups;
        }

        public IEnumerator<Group> GetEnumerator()
        {
            return groups.GetEnumerator();
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class RegexExtensions
    {
        public static Match Match(this Regex regex, string input)
        {
            return new Match(regex.Exec(input));
        }
    }
}
