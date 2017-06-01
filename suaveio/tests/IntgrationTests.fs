namespace RealWorld.Tests
  module Integration =
    open Xunit

    [<Fact>]
    let ``test it dawg``() =
      Assert.True(4 = 1)

    let ``just anothe test``() =
      Assert.True(1 = 1)

    let ``who needs the flash``() =
      Assert.True(3 = 3)
