using RaikesSimplexService.DuckTheSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using RaikesSimplexService.DataModel;
using RaikesSimplexService.DuckTheSystem;
using MathNet.Numerics.LinearAlgebra;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for SolverTest and is intended
    ///to contain all SolverTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SolverTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Solve
        ///</summary>
     [TestMethod]
     public void ExampleSolveTest()
        {
            #region Arrange           

            var lc1 = new LinearConstraint()
            {
                Coefficients = new double[2] { 8, 12 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 24
            };

            var lc2 = new LinearConstraint()
            {
                Coefficients = new double[2] { 12, 12 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 36
            };

            var lc3 = new LinearConstraint()
            {
                Coefficients = new double[2] { 2, 1 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 4
            };

            var lc4 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 5
            };

            var constraints = new List<LinearConstraint>() {lc1, lc2, lc3, lc4};

            var goal = new Goal() 
            { 
                Coefficients = new double[2] { 0.2, 0.3 },
                ConstantTerm = 0
            };           

            var model = new Model()
            {
                Constraints = constraints,
                Goal = goal,
                GoalKind = GoalKind.Minimize
            };
 

            var expected = new Solution()
            {
                Decisions = new double[2] { 3, 0 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 0.6
            };
            #endregion

            //Act

            var solver = new Solver();
            var actual = solver.Solve(model);

            #region Print lines
            System.Diagnostics.Debug.WriteLine(model.DuckString(true));
            System.Diagnostics.Debug.WriteLine(solver.DuckString());
            System.Diagnostics.Debug.WriteLine(actual.DuckString());
            #endregion

            //Assert
            CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
            Assert.AreEqual(expected.Quality, actual.Quality);
            Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
        }

       [TestMethod]
        public void Test1()
        {
            var constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 1, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 1
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 2,-1 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 1
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 0,3 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 2
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 1, 0 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 0
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 0, 1 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 0
            });
            var goal = new Goal()
            {
                Coefficients = new double[] { 6,3 },
                ConstantTerm = 0
            };
            var model = new Model()
            {
                Constraints = constraints,
                Goal = goal,
                GoalKind = GoalKind.Maximize
            };
            var expected = new Solution()
            {
                Decisions = new double[2] { 1, 0 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 6
            };
            var solver = new Solver();
            var actual = solver.Solve(model);

            #region Print lines
            System.Diagnostics.Debug.WriteLine(model.DuckString(true));
            System.Diagnostics.Debug.WriteLine(solver.DuckString());
            System.Diagnostics.Debug.WriteLine(actual.DuckString());
            #endregion

            CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
            Assert.AreEqual(expected.Quality, actual.Quality);
            Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
        }

       [TestMethod]
       public void Test2()
       {
           var constraints = new List<LinearConstraint>();
           constraints.Add(new LinearConstraint()
           {
               Coefficients = new double[] { 1, 1, 1 },
               Relationship = Relationship.LessThanOrEquals,
               Value = 40
           });
           constraints.Add(new LinearConstraint()
           {
               Coefficients = new double[] { 2, 1, -1 },
               Relationship = Relationship.GreaterThanOrEquals,
               Value = 10
           });
           constraints.Add(new LinearConstraint()
           {
               Coefficients = new double[] { 0, -1, 1 },
               Relationship = Relationship.GreaterThanOrEquals,
               Value = 10
           });
           constraints.Add(new LinearConstraint()
           {
               Coefficients = new double[] { 1, 0, 0 },
               Relationship = Relationship.GreaterThanOrEquals,
               Value = 0
           });
           constraints.Add(new LinearConstraint()
           {
               Coefficients = new double[] { 0, 1, 0 },
               Relationship = Relationship.GreaterThanOrEquals,
               Value = 0
           });
           constraints.Add(new LinearConstraint()
           {
               Coefficients = new double[] { 0, 0, 1 },
               Relationship = Relationship.GreaterThanOrEquals,
               Value = 0
           });
           var goal = new Goal()
           {
               Coefficients = new double[] { 2, 3, 1 },
               ConstantTerm = 0
           };
           var model = new Model()
           {
               Constraints = constraints,
               Goal = goal,
               GoalKind = GoalKind.Maximize
           };
           var expected = new Solution()
           {
               Decisions = new double[3] { 10, 10, 20 },
               Quality = SolutionQuality.Optimal,
               AlternateSolutionsExist = false,
               OptimalValue = 70
           };
           var solver = new Solver();
           var actual = solver.Solve(model);

           #region Print lines
           System.Diagnostics.Debug.WriteLine(model.DuckString(true));
           System.Diagnostics.Debug.WriteLine(solver.DuckString());
           System.Diagnostics.Debug.WriteLine(actual.DuckString());
           #endregion

           CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
           Assert.AreEqual(expected.Quality, actual.Quality);
           Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
       }

       [TestMethod]
       public void SinglePhaseTest()
       {
           var constraints = new List<LinearConstraint>();
           constraints.Add(new LinearConstraint()
           {
               Coefficients = new double[] { 10, 5 },
               Relationship = Relationship.LessThanOrEquals,
               Value = 50
           });
           constraints.Add(new LinearConstraint()
           {
               Coefficients = new double[] { 6, 6},
               Relationship = Relationship.LessThanOrEquals,
               Value = 36
           });
           constraints.Add(new LinearConstraint()
           {
               Coefficients = new double[] { 4.5, 18},
               Relationship = Relationship.LessThanOrEquals,
               Value = 81
           });
           var goal = new Goal()
           {
               Coefficients = new double[] { 9, 7},
               ConstantTerm = 0
           };
           var model = new Model()
           {
               Constraints = constraints,
               Goal = goal,
               GoalKind = GoalKind.Maximize
           };
           var expected = new Solution()
           {
               Decisions = new double[2] { 4, 2 },
               Quality = SolutionQuality.Optimal,
               AlternateSolutionsExist = false,
               OptimalValue = 50
           };
           var solver = new Solver();
           var actual = solver.Solve(model);

           #region Print lines
           System.Diagnostics.Debug.WriteLine(model.DuckString(true));
           System.Diagnostics.Debug.WriteLine(solver.DuckString());
           System.Diagnostics.Debug.WriteLine(actual.DuckString());
           #endregion

           CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
           Assert.AreEqual(expected.Quality, actual.Quality);
           Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
       }

       [TestMethod]
       public void SinglePhaseTest2()
       {
           var constraints = new List<LinearConstraint>();
           constraints.Add(new LinearConstraint()
           {
               Coefficients = new double[] { 1,1 },
               Relationship = Relationship.LessThanOrEquals,
               Value = 35
           });
           constraints.Add(new LinearConstraint()
           {
               Coefficients = new double[] { 1,2 },
               Relationship = Relationship.LessThanOrEquals,
               Value = 38
           });
           constraints.Add(new LinearConstraint()
           {
               Coefficients = new double[] { 2,2 },
               Relationship = Relationship.LessThanOrEquals,
               Value = 50
           });
           var goal = new Goal()
           {
               Coefficients = new double[] { 350, 450 },
               ConstantTerm = 0
           };
           var model = new Model()
           {
               Constraints = constraints,
               Goal = goal,
               GoalKind = GoalKind.Maximize
           };
           var expected = new Solution()
           {
               Decisions = new double[2] { 12,13 },
               Quality = SolutionQuality.Optimal,
               AlternateSolutionsExist = false,
               OptimalValue = 10050
           };
           var solver = new Solver();
           var actual = solver.Solve(model);

           #region Print lines
           System.Diagnostics.Debug.WriteLine(model.DuckString(true));
           System.Diagnostics.Debug.WriteLine(solver.DuckString());
           System.Diagnostics.Debug.WriteLine(actual.DuckString());
           #endregion

           CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
           Assert.AreEqual(expected.Quality, actual.Quality);
           Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
       }
    }
}
