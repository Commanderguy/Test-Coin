using CoinFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Tests
{
    [TestClass]
    public class CoinFrameworkTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Blockchain chain = new Blockchain();

            AccountView Person1 = new AccountView();
            AccountView Person2 = new AccountView();

            Block nullBlock = new Block();
            nullBlock.block_number = 0;
            nullBlock.calculateNonce(Person1.publicKey, chain);

            chain.AddBlock(nullBlock);

            Assert.IsTrue(chain.validate(), "The chain is invalid, even though it should be valid, as it only contains the nullblock");

            Transaction tx = new Transaction(Person1.publicKey, Person2.publicKey, 30, 0, Person1.privateKey);
            Block b = new Block();
            b.AddTransaction(tx);

            b.block_number = 1;
            b.prev_hash = chain.getLastHash();

            b.calculateNonce(Person1.publicKey, chain);

            chain.AddBlock(b);

            Assert.AreEqual(chain.count_funds(Person1.publicKey), 70, "Person one has a wrong number of coins");
            Assert.AreEqual(chain.count_funds(Person2.publicKey), 30, "Person one has a wrong number of coins");
            

        }
    }
}
