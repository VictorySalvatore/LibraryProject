using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LibraryTests
{
    [TestFixture]
    public class LibraryManagerTests
    {
        private LibraryManager m;
        [SetUp]
        public void Setup()
        {
            m = new LibraryManager();
            m.InitializeDefaultData();
        }
        [Test]
        public void AddBookDuplicateUdc()
        {
            Book b = new Book("004.4", "X", "Y", 2020, 1);
            Assert.Throws<LibraryException>(() => m.AddBook(b));
        }

        [Test]
        public void IssueLoanDecreasesAvailable()
        {
            int before = m.FindBook("53.01").Now;
            m.IssueLoan("53.01", 1);
            Assert.That(m.FindBook("53.01").Now, Is.EqualTo(before - 1));
        }
        [Test]
        public void ReturnLoanIncreasesAvailable()
        {
            Loan loan = m.IssueLoan("82-31", 1);
            m.ReturnLoan(loan.Id);
            Assert.That(m.FindBook("82-31").Now, Is.EqualTo(4));
        }
        [Test]
        public void ReturnReturnedLoan()
        {
            Loan loan = m.IssueLoan("82-31", 1);
            m.ReturnLoan(loan.Id);
            Assert.Throws<LibraryException>(() => m.ReturnLoan(loan.Id));
        }
        [Test]
        public void IssueLoanWhenNoCopies()
        {
            m.IssueLoan("61", 1);
            Assert.Throws<LibraryException>(() => m.IssueLoan("61", 2));
        }
        [Test]
        public void IssueLoanToBlockedReader()
        {
            Reader r = m.FindReader(1);
            r.IsActive = false;
            Assert.Throws<LibraryException>(() => m.IssueLoan("517.2", r.Id));
        }
        [Test]
        public void PenaltyIsZero()
        {
            Loan loan = m.IssueLoan("53.01", 2, 14);
            decimal pen = m.Penalty(loan.Id);
            Assert.That(pen, Is.EqualTo(0));
        }
        [Test]
        public void PenaltyIsPositive()
        {
            Loan loan = m.IssueLoan("53.01", 2, -5);
            decimal pen = m.Penalty(loan.Id);
            Assert.That(pen, Is.GreaterThan(0));
        }
    }
}
