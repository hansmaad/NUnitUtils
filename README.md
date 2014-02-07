# NUnitUtils

## LikeConstraint
Some Examples

    class Foo
    {
        public Foo(int i) { I = i; }
        private int I { get; set; }
        public int Number { get; set; }
        public string Text { get; set; }
        public Bar B { get; set; }
    };

    [Test]
    public void PublicProperties_True()
    {
        PublicProperties a = new PublicProperties(1)
        {
            Number = 1,
            Text = "guru",
            B = new Bar()
        };
        PublicProperties b = new PublicProperties(2)
        {
            Number = 1,
            Text = "guru",
            B = new Bar()
        };

        Assert.That(a, Looks.Like(b));  // fails a.B != b.B
        Assert.That(a, Looks.Like(b).Without("B"));           // pass
        Assert.That(a, Looks.Like(b).Without(x => x.B));      // pass
        Assert.That(a, Looks.Like(b).WithoutReferenceTypes);  // pass
    }
