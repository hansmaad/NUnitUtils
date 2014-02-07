using NUnit.Framework;
using NunitUtils;

namespace NUnitUtils.Constraints
{
    [TestFixture]
    class LikeConstraintTest
    {
        class NoProperties
        {
            public NoProperties(int i)
            {
                this.i = i;
            }
            internal int i;
        };


        [Test]
        public void NoProperties_True()
        {
            NoProperties a = new NoProperties(1);
            NoProperties b = new NoProperties(2);

            var like = Looks.Like(a);
            Assert.That(like.Matches(b), Is.True);
        }

        class NoPublicProperties
        {
            public NoPublicProperties(int i)
            {
                I = i;
            }
            private int I { get; set; }
        };

        [Test]
        public void NoPublicProperties_True()
        {
            NoPublicProperties a = new NoPublicProperties(1);
            NoPublicProperties b = new NoPublicProperties(2);

            var like = Looks.Like(a);
            Assert.That(like.Matches(b), Is.True);
        }

        class PublicProperties
        {
            public PublicProperties(int i)
            {
                I = i;
            }
            private int I { get; set; }

            public int Number { get; set; }

            public string Text { get; set; }

            public NoProperties Foo { get; set; }
        };

        [Test]
        public void PublicProperties_True()
        {
            PublicProperties a = new PublicProperties(1)
            {
                Number = 1,
                Text = "guru",
                Foo = new NoProperties(1)
            };
            PublicProperties b = new PublicProperties(2)
            {
                Number = 1,
                Text = "guru",
                Foo = a.Foo
            };

            var like = Looks.Like(a);
            Assert.That(like.Matches(b), Is.True);
        }

        [Test]
        public void PublicProperties_False()
        {
            PublicProperties a = new PublicProperties(1)
            {
                Number = 1,
                Text = "guru",
                Foo = new NoProperties(1)
            };
            PublicProperties b = new PublicProperties(2)
            {
                Number = 2,
                Text = "guru2",
                Foo = new NoProperties(1)
            };

            var like = Looks.Like(a);
            Assert.That(like.Matches(b), Is.False);
        }

        [Test]
        public void PublicProperties_HasNull_False()
        {
            PublicProperties a = new PublicProperties(1)
            {
                Number = 1,
                Text = "guru",
                Foo = null
            };
            PublicProperties b = new PublicProperties(2)
            {
                Number = 1,
                Text = "guru",
                Foo = new NoProperties(1)
            };

            var like = Looks.Like(a);
            Assert.That(like.Matches(b), Is.False);
        }

        [Test]
        public void PublicProperties_BothNull_True()
        {
            PublicProperties a = new PublicProperties(1)
            {
                Number = 1,
                Text = "guru",
                Foo = null
            };
            PublicProperties b = new PublicProperties(2)
            {
                Number = 1,
                Text = "guru",
                Foo = null
            };
            var like = Looks.Like(a);
            Assert.That(like.Matches(b), Is.True, like.Message);
        }

        [Test]
        public void Without_String_True()
        {
            PublicProperties a = new PublicProperties(1)
            {
                Number = 1,
                Text = "guru",
                Foo = null
            };
            PublicProperties b = new PublicProperties(2)
            {
                Number = 1,
                Text = "guru",
                Foo = new NoProperties(1)
            };
            var like = Looks.Like(a).Without("Pump");
            Assert.That(like.Matches(b), Is.True, like.Message);
        }

        [Test]
        public void Without_Expression_True()
        {
            PublicProperties a = new PublicProperties(1)
            {
                Number = 1,
                Text = "guru",
                Foo = null
            };
            PublicProperties b = new PublicProperties(2)
            {
                Number = 1,
                Text = "guru",
                Foo = new NoProperties(1)
            };
            var like = Looks.Like(a).Without(p => p.Foo);
            Assert.That(like.Matches(b), Is.True, like.Message);
        }

        [Test]
        public void WithoutRefTypes_Expression_True()
        {
            PublicProperties a = new PublicProperties(1)
            {
                Number = 1,
                Text = "guru",
                Foo = new NoProperties(1)
            };
            PublicProperties b = new PublicProperties(2)
            {
                Number = 1,
                Text = "guru",
                Foo = new NoProperties(1)
            };
            var like = Looks.Like(a).WithoutReferenceTypes();
            Assert.That(like.Matches(b), Is.True, like.Message);
        }



        class PublicProperties2
        {
            public int Number { get; set; }

            public string Text { get; set; }
        };

        [Test]
        public void PublicProperties_DifferentTypes()
        {
            var a = new PublicProperties(1)
            {
                Number = 1,
                Text = "guru",
                Foo = new NoProperties(1)
            };

            var b = new PublicProperties2()
            {
                Number = 1,
                Text = "guru"
            };
            var like1 = Looks.Like(a);
            var like2 = Looks.Like(b);
            Assert.That(like1.Matches(b), Is.True, like1.Message);
            Assert.That(like2.Matches(a), Is.True, like2.Message);
        }
    }
}
