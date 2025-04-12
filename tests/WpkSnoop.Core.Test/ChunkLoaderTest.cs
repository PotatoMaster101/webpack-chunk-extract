using System.Collections;
using Acornima;

namespace WpkSnoop.Core.Test;

/// <summary>
/// Unit tests for <see cref="ChunkLoader"/>.
/// </summary>
public class ChunkLoaderTest
{
    [Theory]
    [ClassData(typeof(GetIdsTestData))]
    public void GetIds_ReturnsCorrectValue(string js, ChunkId[] expected)
    {
        // arrange
        var parser = new Parser();
        var module = parser.ParseModule(js);
        var loaders = ChunkLoaderHelper.GetChunkLoaders(module);

        // act
        var result = loaders.SelectMany(x => x.GetIds()).ToList();

        // assert
        Assert.Equal(expected.Length, result.Count);
        for (var i = 0; i < expected.Length; i++)
            Assert.Equal(expected[i], result[i]);
    }

    [Theory]
    [ClassData(typeof(ExecuteTestData))]
    public void Execute_ReturnsCorrectValue(string js, ChunkEntry[] expected)
    {
        // arrange
        var parser = new Parser();
        var module = parser.ParseModule(js);
        var loaders = ChunkLoaderHelper.GetChunkLoaders(module);

        // act
        var result = loaders.SelectMany(x => x.Execute(x.GetIds())).ToList();

        // assert
        Assert.Equal(expected.Length, result.Count);
        for (var i = 0; i < expected.Length; i++)
            Assert.Equal(expected[i], result[i]);
    }

    /// <summary>
    /// Test data for <see cref="ChunkLoaderTest.GetIds_ReturnsCorrectValue"/>.
    /// </summary>
    private class GetIdsTestData : IEnumerable<object[]>
    {
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return
            [
                File.ReadAllText("Samples/array-lookup.js"), new ChunkId[]
                {
                    new("0", false),
                    new("1", false),
                    new("2", false),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/big.js"), new ChunkId[]
                {
                    new("24", false), new("84", false), new("160", false),
                    new("182", false), new("195", false), new("215", false),
                    new("243", false), new("278", false), new("284", false),
                    new("308", false), new("319", false), new("370", false),
                    new("458", false), new("498", false), new("541", false),
                    new("558", false), new("685", false), new("786", false),
                    new("830", false), new("908", false), new("951", false),
                    new("996", false), new("1011", false), new("1019", false),
                    new("1158", false), new("1331", false), new("1426", false),
                    new("1459", false), new("1488", false), new("1503", false),
                    new("1514", false), new("1596", false), new("1698", false),
                    new("1782", false), new("1975", false), new("2012", false),
                    new("2052", false), new("2074", false), new("2084", false),
                    new("2091", false), new("2094", false), new("2133", false),
                    new("2157", false), new("2203", false), new("2311", false),
                    new("2352", false), new("2463", false), new("2499", false),
                    new("2511", false), new("2562", false), new("2590", false),
                    new("2703", false), new("2928", false), new("3049", false),
                    new("3140", false), new("3162", false), new("3216", false),
                    new("3245", false), new("3315", false), new("3335", false),
                    new("3352", false), new("3367", false), new("3423", false),
                    new("3438", false), new("3459", false), new("3479", false),
                    new("3507", false), new("3574", false), new("3684", false),
                    new("3707", false), new("3713", false), new("3819", false),
                    new("3831", false), new("3911", false), new("3994", false),
                    new("4013", false), new("4024", false), new("4100", false),
                    new("4147", false), new("4192", false), new("4281", false),
                    new("4321", false), new("4334", false), new("4598", false),
                    new("4613", false), new("4708", false), new("4819", false),
                    new("4858", false), new("4859", false), new("4946", false),
                    new("5051", false), new("5143", false), new("5190", false),
                    new("5209", false), new("5332", false), new("5335", false),
                    new("5355", false), new("5357", false), new("5431", false),
                    new("5457", false), new("5566", false), new("5570", false),
                    new("5646", false), new("5823", false), new("5837", false),
                    new("5865", false), new("5918", false), new("5924", false),
                    new("6083", false), new("6149", false), new("6221", false),
                    new("6263", false), new("6334", false), new("6502", false),
                    new("6546", false), new("6561", false), new("6611", false),
                    new("6619", false), new("6620", false), new("6688", false),
                    new("6693", false), new("6951", false), new("7008", false),
                    new("7031", false), new("7119", false), new("7126", false),
                    new("7250", false), new("7438", false), new("7443", false),
                    new("7470", false), new("7546", false), new("7594", false),
                    new("7660", false), new("7992", false), new("8022", false),
                    new("8070", false), new("8099", false), new("8104", false),
                    new("8210", false), new("8212", false), new("8244", false),
                    new("8266", false), new("8408", false), new("8436", false),
                    new("8640", false), new("8691", false), new("8768", false),
                    new("8777", false), new("8807", false), new("8829", false),
                    new("9023", false), new("9080", false), new("9146", false),
                    new("9200", false), new("9264", false), new("9329", false),
                    new("9373", false), new("9515", false), new("9537", false),
                    new("9585", false), new("9633", false), new("9727", false),
                    new("9770", false), new("9805", false), new("9874", false),
                    new("9912", false)
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/field-ref.js"), new ChunkId[]
                {
                    new("0", false), new("1", false), new("2", false), new("3", false),
                    new("4", false), new("5", false), new("6", false), new("7", false),
                    new("8", false),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/func-declare.js"), new ChunkId[]
                {
                    new("1"), new("2"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/has-source-map.js"), new ChunkId[]
                {
                    new("400", false), new("411", false), new("826", false),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/ifs.js"), new ChunkId[]
                {
                    new("chunk1"), new("411"), new("826", false),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/inf-loop.js"), new ChunkId[]
                {
                    new("400", false), new("411", false), new("826", false),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/mix-ternary-map.js"), new ChunkId[]
                {
                    new("987", false), new("925", false), new("271", false), new("351", false), new("549", false),
                    new("551", false), new("553", false), new("899", false),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/multi-field-ref.js"), new ChunkId[]
                {
                    new("0", false), new("1", false), new("2", false), new("3", false), new("4", false),
                    new("5", false), new("6", false), new("7", false), new("8", false),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/multi-roots.js"), new ChunkId[]
                {
                    new("136", false), new("150", false), new("317", false), new("428", false), new("625", false),
                    new("766", false), new("848", false),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/no-minification.js"), new ChunkId[]
                {
                    new("400"), new("411"), new("826"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/no-numeric-ids.js"), new ChunkId[]
                {
                    new("chunk1"), new("chunk2"), new("src_modules_mod3_js"), new("chunkBtn"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/small.js"), new ChunkId[]
                {
                    new("400", false), new("411", false), new("826", false),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/switch.js"), new ChunkId[]
                {
                    new("chunk1"), new("411"), new("826", false),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/ternary.js"), new ChunkId[]
                {
                    new("chunk1"), new("411", false), new("826", false),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/two-loaders.js"), new ChunkId[]
                {
                    new("400", false), new("411", false), new("826", false), new("400", false), new("411", false),
                    new("826", false),
                }
            ];
        }
    }

    /// <summary>
    /// Test data for <see cref="ChunkLoaderTest.Execute_ReturnsCorrectValue"/>.
    /// </summary>
    private class ExecuteTestData : IEnumerable<object[]>
    {
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<object[]>
        GetEnumerator()
        {
            yield return
            [
                File.ReadAllText("Samples/array-lookup.js"), new ChunkEntry[]
                {
                    new(new ChunkId("0", false), "chunk-0.js"),
                    new(new ChunkId("1", false), "chunk-1.js"),
                    new(new ChunkId("2", false), "chunk-2.js"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/big.js"), new ChunkEntry[]
                {
                    new(new ChunkId("24", false), "24.6d7203d5e7dab233.js"),
                    new(new ChunkId("84", false), "84.67087adef6f18025.js"),
                    new(new ChunkId("160", false), "160.99255e9b984eeada.js"),
                    new(new ChunkId("182", false), "182.1e2d9aede1417d20.js"),
                    new(new ChunkId("195", false), "195.7edd03b55d5963e3.js"),
                    new(new ChunkId("215", false), "215.8627b81c7185c418.js"),
                    new(new ChunkId("243", false), "243.e3bdd72360c84809.js"),
                    new(new ChunkId("278", false), "278.3e49b8681909f2d2.js"),
                    new(new ChunkId("284", false), "284.bd0cfda6ce9d91d5.js"),
                    new(new ChunkId("308", false), "308.341144a43764ceff.js"),
                    new(new ChunkId("319", false), "319.0a30afed78020bc0.js"),
                    new(new ChunkId("370", false), "370.727b5fd0b3ec3340.js"),
                    new(new ChunkId("458", false), "458.3327ee5c48f901f2.js"),
                    new(new ChunkId("498", false), "498.4245721945c953bd.js"),
                    new(new ChunkId("541", false), "541.60cf5dbabb594de6.js"),
                    new(new ChunkId("558", false), "558.c26d6abee0f8a4f0.js"),
                    new(new ChunkId("685", false), "685.673f8c1a35efbef8.js"),
                    new(new ChunkId("786", false), "786.3c099d4fe278bf30.js"),
                    new(new ChunkId("830", false), "830.fa70f987cfda2e8b.js"),
                    new(new ChunkId("908", false), "908.6f2d90361e2b6fb4.js"),
                    new(new ChunkId("951", false), "951.8986f2301d3b8884.js"),
                    new(new ChunkId("996", false), "996.9969caa925eaa442.js"),
                    new(new ChunkId("1011", false), "1011.c4fa08b4222aeec3.js"),
                    new(new ChunkId("1019", false), "1019.f3391044a84e8a2b.js"),
                    new(new ChunkId("1158", false), "1158.3eae60541c3dbc96.js"),
                    new(new ChunkId("1331", false), "1331.a8eb0889f102da21.js"),
                    new(new ChunkId("1426", false), "1426.93cd9d2375805672.js"),
                    new(new ChunkId("1459", false), "1459.3821e94f7b6384ad.js"),
                    new(new ChunkId("1488", false), "1488.43f84f0eaa0a4f30.js"),
                    new(new ChunkId("1503", false), "1503.3098c00e286756fc.js"),
                    new(new ChunkId("1514", false), "1514.69c3c453ecb6ce36.js"),
                    new(new ChunkId("1596", false), "1596.bed83e77202ac8bd.js"),
                    new(new ChunkId("1698", false), "1698.0efeb12e1c0ec686.js"),
                    new(new ChunkId("1782", false), "1782.b2ead526ffab06e5.js"),
                    new(new ChunkId("1975", false), "1975.e9fe58910cf28fc9.js"),
                    new(new ChunkId("2012", false), "2012.d04047a3f746a6ef.js"),
                    new(new ChunkId("2052", false), "2052.938ecfeb6610ac6d.js"),
                    new(new ChunkId("2074", false), "2074.1fd2067a21eb65c0.js"),
                    new(new ChunkId("2084", false), "2084.6d9345f837f0b408.js"),
                    new(new ChunkId("2091", false), "2091.9224797a66700195.js"),
                    new(new ChunkId("2094", false), "2094.498012ab4acd2dfe.js"),
                    new(new ChunkId("2133", false), "2133.853ee8ce8fbb2654.js"),
                    new(new ChunkId("2157", false), "2157.6326f2f2c61af97b.js"),
                    new(new ChunkId("2203", false), "2203.bbf7fa7402fbca54.js"),
                    new(new ChunkId("2311", false), "2311.aff741027b97231c.js"),
                    new(new ChunkId("2352", false), "2352.4d4e5a1b662d4f25.js"),
                    new(new ChunkId("2463", false), "2463.0fb7e85b5b53ac66.js"),
                    new(new ChunkId("2499", false), "2499.f93e43143c8bd88b.js"),
                    new(new ChunkId("2511", false), "2511.3514bde2cb0a0ef9.js"),
                    new(new ChunkId("2562", false), "2562.fc43e8e6c60b6582.js"),
                    new(new ChunkId("2590", false), "2590.be217f8ac4936973.js"),
                    new(new ChunkId("2703", false), "2703.0516506aaa87937a.js"),
                    new(new ChunkId("2928", false), "2928.aa823fbebf01abc3.js"),
                    new(new ChunkId("3049", false), "3049.a20c92ba8af9c56c.js"),
                    new(new ChunkId("3140", false), "3140.6a1a625e88b592f9.js"),
                    new(new ChunkId("3162", false), "3162.8f021e022ae14ff8.js"),
                    new(new ChunkId("3216", false), "3216.6e1d5dc12e05ad8b.js"),
                    new(new ChunkId("3245", false), "3245.7899ba23ee9944ec.js"),
                    new(new ChunkId("3315", false), "3315.e1e3c424a92d74a4.js"),
                    new(new ChunkId("3335", false), "3335.d50ca3242419d54d.js"),
                    new(new ChunkId("3352", false), "3352.2f0419f7516dd24c.js"),
                    new(new ChunkId("3367", false), "3367.15a1b6d3a7b8cb64.js"),
                    new(new ChunkId("3423", false), "3423.4745b310df7666e1.js"),
                    new(new ChunkId("3438", false), "3438.d795838ec2ae94ec.js"),
                    new(new ChunkId("3459", false), "3459.cdd9f24496603532.js"),
                    new(new ChunkId("3479", false), "3479.5069e4ad8a5f14a1.js"),
                    new(new ChunkId("3507", false), "3507.1bb71b608afd62a6.js"),
                    new(new ChunkId("3574", false), "3574.65c68260b7e2f72a.js"),
                    new(new ChunkId("3684", false), "3684.8b8441739d67e527.js"),
                    new(new ChunkId("3707", false), "3707.cb62e27bb68ccd9d.js"),
                    new(new ChunkId("3713", false), "3713.a5835b477a0f855e.js"),
                    new(new ChunkId("3819", false), "3819.b45e1430f62b83e5.js"),
                    new(new ChunkId("3831", false), "3831.830e54de77e46e88.js"),
                    new(new ChunkId("3911", false), "3911.4cd957e833074f75.js"),
                    new(new ChunkId("3994", false), "3994.e7597ee796e90b47.js"),
                    new(new ChunkId("4013", false), "4013.8023c2e962bcf026.js"),
                    new(new ChunkId("4024", false), "4024.70ee01e0c07633e7.js"),
                    new(new ChunkId("4100", false), "4100.2593bb5f24645e8c.js"),
                    new(new ChunkId("4147", false), "4147.f75eed319d031198.js"),
                    new(new ChunkId("4192", false), "4192.8eaafb17ff63d05c.js"),
                    new(new ChunkId("4281", false), "4281.37ba091b7a93219d.js"),
                    new(new ChunkId("4321", false), "4321.ae4c647d70813dfc.js"),
                    new(new ChunkId("4334", false), "4334.61efd645c7e0d37b.js"),
                    new(new ChunkId("4598", false), "4598.bce5fa081241ba97.js"),
                    new(new ChunkId("4613", false), "4613.a24e6536b15573cc.js"),
                    new(new ChunkId("4708", false), "4708.9e94df6fa4bdc931.js"),
                    new(new ChunkId("4819", false), "4819.61f2bbcc738b35ad.js"),
                    new(new ChunkId("4858", false), "4858.78201e7da25e1224.js"),
                    new(new ChunkId("4859", false), "4859.fb80fcc2ec076dbe.js"),
                    new(new ChunkId("4946", false), "4946.fd03a6766f28a3ac.js"),
                    new(new ChunkId("5051", false), "5051.ba3e9ffdd070652c.js"),
                    new(new ChunkId("5143", false), "5143.5c6bd9fc3dfac44e.js"),
                    new(new ChunkId("5190", false), "5190.ab95444983dddda1.js"),
                    new(new ChunkId("5209", false), "5209.875c0867b3aeb957.js"),
                    new(new ChunkId("5332", false), "5332.92debffec51a87d5.js"),
                    new(new ChunkId("5335", false), "5335.34b7c71209977a94.js"),
                    new(new ChunkId("5355", false), "5355.d0c27135f0808d30.js"),
                    new(new ChunkId("5357", false), "5357.5a95258cf76a101f.js"),
                    new(new ChunkId("5431", false), "5431.74099023d4f2d9b2.js"),
                    new(new ChunkId("5457", false), "5457.45e344139ef91366.js"),
                    new(new ChunkId("5566", false), "5566.37e216e6d9993863.js"),
                    new(new ChunkId("5570", false), "5570.c04e79738b244d57.js"),
                    new(new ChunkId("5646", false), "5646.9da17b854c906902.js"),
                    new(new ChunkId("5823", false), "5823.fcea6a578f8d1f4f.js"),
                    new(new ChunkId("5837", false), "5837.572266fa4083900d.js"),
                    new(new ChunkId("5865", false), "5865.1f0829f911361d7e.js"),
                    new(new ChunkId("5918", false), "5918.0e0ac2ab09a26b8b.js"),
                    new(new ChunkId("5924", false), "5924.f195543afb7a5eb8.js"),
                    new(new ChunkId("6083", false), "6083.2fdbe24754c80004.js"),
                    new(new ChunkId("6149", false), "6149.d871f88ccb802cad.js"),
                    new(new ChunkId("6221", false), "6221.e63b103b8f66ab07.js"),
                    new(new ChunkId("6263", false), "6263.97f892c4049a3222.js"),
                    new(new ChunkId("6334", false), "6334.59d996d25014d795.js"),
                    new(new ChunkId("6502", false), "6502.a8c6987b5232ac6d.js"),
                    new(new ChunkId("6546", false), "6546.ac155798590ed453.js"),
                    new(new ChunkId("6561", false), "6561.07862d5632914a48.js"),
                    new(new ChunkId("6611", false), "6611.65baa3231e2ce456.js"),
                    new(new ChunkId("6619", false), "6619.140e5d0f225fd971.js"),
                    new(new ChunkId("6620", false), "6620.22f75944e1084abf.js"),
                    new(new ChunkId("6688", false), "6688.7eded9562469be85.js"),
                    new(new ChunkId("6693", false), "6693.cdba44423878f3c9.js"),
                    new(new ChunkId("6951", false), "6951.95e534b59544ca9a.js"),
                    new(new ChunkId("7008", false), "7008.e1929c00bab1dc34.js"),
                    new(new ChunkId("7031", false), "7031.7aedd0ad5100b56c.js"),
                    new(new ChunkId("7119", false), "7119.f9dfa6c6c4b6a864.js"),
                    new(new ChunkId("7126", false), "7126.18339ddf4853d925.js"),
                    new(new ChunkId("7250", false), "7250.0363a3fea9944b5c.js"),
                    new(new ChunkId("7438", false), "7438.b62fccfdb61d907b.js"),
                    new(new ChunkId("7443", false), "7443.ee0a77dff9aaf232.js"),
                    new(new ChunkId("7470", false), "7470.dfccb6134d7bc022.js"),
                    new(new ChunkId("7546", false), "7546.cb6462249689f813.js"),
                    new(new ChunkId("7594", false), "7594.6f4baf3939b18e30.js"),
                    new(new ChunkId("7660", false), "7660.2bf752bca45a4be9.js"),
                    new(new ChunkId("7992", false), "7992.157eae5cd1bfb012.js"),
                    new(new ChunkId("8022", false), "8022.72d49fba7a1e9c34.js"),
                    new(new ChunkId("8070", false), "8070.180c21ea9a0e49b9.js"),
                    new(new ChunkId("8099", false), "8099.b53d53bc831801a1.js"),
                    new(new ChunkId("8104", false), "8104.cffae17eecbcad95.js"),
                    new(new ChunkId("8210", false), "8210.9a02c38672308ed3.js"),
                    new(new ChunkId("8212", false), "8212.a5906adb0dfd104d.js"),
                    new(new ChunkId("8244", false), "8244.5b1641734ff044f9.js"),
                    new(new ChunkId("8266", false), "8266.93b57075941bf7b3.js"),
                    new(new ChunkId("8408", false), "8408.154a74952e36d19b.js"),
                    new(new ChunkId("8436", false), "8436.5a195e6fffc6bf03.js"),
                    new(new ChunkId("8640", false), "8640.44adfc6b158e3f10.js"),
                    new(new ChunkId("8691", false), "8691.84166f540df0c61c.js"),
                    new(new ChunkId("8768", false), "8768.a1d3fd00bea2a681.js"),
                    new(new ChunkId("8777", false), "8777.009dd40d3b7f7e2b.js"),
                    new(new ChunkId("8807", false), "8807.22b2b56d235ea31b.js"),
                    new(new ChunkId("8829", false), "8829.4a5a723bed445b72.js"),
                    new(new ChunkId("9023", false), "9023.e3ed827dc6d0a0a9.js"),
                    new(new ChunkId("9080", false), "9080.01dfd5d4fdfef06e.js"),
                    new(new ChunkId("9146", false), "9146.d939f60ee5c5e8be.js"),
                    new(new ChunkId("9200", false), "9200.91305a18bb841775.js"),
                    new(new ChunkId("9264", false), "9264.c5b06020c03e1715.js"),
                    new(new ChunkId("9329", false), "9329.a2ec12f90f2b9d22.js"),
                    new(new ChunkId("9373", false), "9373.14b1065d4068c5cf.js"),
                    new(new ChunkId("9515", false), "9515.6307b0b2cca41620.js"),
                    new(new ChunkId("9537", false), "9537.ab43f8ddb924335d.js"),
                    new(new ChunkId("9585", false), "9585.a751fd306fc6c687.js"),
                    new(new ChunkId("9633", false), "9633.ce2fe8513816d968.js"),
                    new(new ChunkId("9727", false), "9727.b7f9e31c8311988f.js"),
                    new(new ChunkId("9770", false), "9770.add5b047f4a35bb0.js"),
                    new(new ChunkId("9805", false), "9805.4fe32b4bda0a0b08.js"),
                    new(new ChunkId("9874", false), "9874.74d17c3416ef72e7.js"),
                    new(new ChunkId("9912", false), "9912.297c0dd588eaf72d.js"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/field-ref.js"), new ChunkEntry[]
                {
                    new(new ChunkId("0", false), "chunks/ccpa.36fdb28032f45d0673c2.bundle.js"),
                    new(new ChunkId("1", false), "chunks/ccpa-gpp.59db5f353838b3001f26.bundle.js"),
                    new(new ChunkId("2", false), "chunks/custom.d67ce0f1f4b3bd4afdd0.bundle.js"),
                    new(new ChunkId("3", false), "chunks/gdpr.f04e9655e40809d62483.bundle.js"),
                    new(new ChunkId("4", false), "chunks/gdpr-tcf.3a9316ed6cca6bbd4e92.bundle.js"),
                    new(new ChunkId("5", false), "chunks/hbbtv.124c9a8ad6c1b442fc1b.bundle.js"),
                    new(new ChunkId("6", false), "chunks/preferences.d79ae5e4e6fd5f0c0216.bundle.js"),
                    new(new ChunkId("7", false), "chunks/usnat.9347e790845aa57c6c24.bundle.js"),
                    new(new ChunkId("8", false), "chunks/usnat-uspapi.92131c9e73e4e75ba068.bundle.js"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/func-declare.js"), new ChunkEntry[]
                {
                    new(new ChunkId("1"), "chunk1.js"),
                    new(new ChunkId("2"), "chunk2.js"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/has-source-map.js"), new ChunkEntry[]
                {
                    new(new ChunkId("400", false), "chunk2.c36ed2dbc7455af56445.js"),
                    new(new ChunkId("411", false), "chunk1.655a7d0d1b477637ba24.js"),
                    new(new ChunkId("826", false), "chunkBtn.19da0f443e99cfe71d67.js"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/ifs.js"), new ChunkEntry[]
                {
                    new(new ChunkId("chunk1"), "5bc7d56beddd675ee02a.js"),
                    new(new ChunkId("411"), "bee97256afba34de130e.js"),
                    new(new ChunkId("826", false), "c2ce9ce9700e2a0ab724.js"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/inf-loop.js"), new ChunkEntry[]
                {
                    new(new ChunkId("400", false), "chunk2.5bc7d56beddd675ee02a.js"),
                    new(new ChunkId("411", false), "chunk1.bee97256afba34de130e.js"),
                    new(new ChunkId("826", false), "chunkBtn.c2ce9ce9700e2a0ab724.js"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/mix-ternary-map.js"), new ChunkEntry[]
                {
                    new(new ChunkId("987", false), "static/chunks/987-06e74253d3d6fdb0.js"),
                    new(new ChunkId("925", false), "static/chunks/925-00c5e93da9c24f8c.js"),
                    new(new ChunkId("271", false), "static/chunks/271.71a95145b3123fb2.js"),
                    new(new ChunkId("351", false), "static/chunks/351.1010f2d05ea9d916.js"),
                    new(new ChunkId("549", false), "static/chunks/549.fa155d24f1aa38e7.js"),
                    new(new ChunkId("551", false), "static/chunks/551.af84707217e582b4.js"),
                    new(new ChunkId("553", false), "static/chunks/553.09fe7409989aed52.js"),
                    new(new ChunkId("899", false), "static/chunks/899.39c6d508fc8d66f8.js"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/multi-field-ref.js"), new ChunkEntry[]
                {
                    new(new ChunkId("0", false), "chunks/js/2ccpa.36fdb28032f45d0673c2.bundle.js"),
                    new(new ChunkId("1", false), "chunks/js/2ccpa-gpp.59db5f353838b3001f26.bundle.js"),
                    new(new ChunkId("2", false), "chunks/js/2custom.d67ce0f1f4b3bd4afdd0.bundle.js"),
                    new(new ChunkId("3", false), "chunks/js/2gdpr.f04e9655e40809d62483.bundle.js"),
                    new(new ChunkId("4", false), "chunks/js/2gdpr-tcf.3a9316ed6cca6bbd4e92.bundle.js"),
                    new(new ChunkId("5", false), "chunks/js/2hbbtv.124c9a8ad6c1b442fc1b.bundle.js"),
                    new(new ChunkId("6", false), "chunks/js/2preferences.d79ae5e4e6fd5f0c0216.bundle.js"),
                    new(new ChunkId("7", false), "chunks/js/2usnat.9347e790845aa57c6c24.bundle.js"),
                    new(new ChunkId("8", false), "chunks/js/2usnat-uspapi.92131c9e73e4e75ba068.bundle.js"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/multi-roots.js"), new ChunkEntry[]
                {
                    new(new ChunkId("136", false), "static/chunks/page-8e28.8334ebfd.js"),
                    new(new ChunkId("150", false), "static/chunks/layout-8ea2.3e1b3a11.js"),
                    new(new ChunkId("317", false), "static/chunks/dynamic-fingerprint.59f13125.js"),
                    new(new ChunkId("428", false), "static/chunks/dynamic-common-widgets.14181a0b.js"),
                    new(new ChunkId("625", false), "static/chunks/loaders.0c2e0be4.js"),
                    new(new ChunkId("766", false), "static/chunks/dynamic-com-widgets.9b1695df.js"),
                    new(new ChunkId("848", false), "static/chunks/dynamic-analytics-web-vitals.7f5ba5ae.js"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/no-minification.js"), new ChunkEntry[]
                {
                    new(new ChunkId("400"), "chunk2.522c174281df6867160f.js"),
                    new(new ChunkId("411"), "chunk1.defd75d4e0e0082175d2.js"),
                    new(new ChunkId("826"), "chunkBtn.6a4d614c9886ec0ba82a.js"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/no-numeric-ids.js"), new ChunkEntry[]
                {
                    new(new ChunkId("chunk1"), "chunk1.f1195e719ca60f6a72c0.js"),
                    new(new ChunkId("chunk2"), "chunk2.465e8bbaa66177c00940.js"),
                    new(new ChunkId("src_modules_mod3_js"), "src_modules_mod3_js.13714958953ee2c9b344.js"),
                    new(new ChunkId("chunkBtn"), "chunkBtn.dca13bd1686fbf6af19b.js"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/small.js"), new ChunkEntry[]
                {
                    new(new ChunkId("400", false), "chunk2.5bc7d56beddd675ee02a.js"),
                    new(new ChunkId("411", false), "chunk1.bee97256afba34de130e.js"),
                    new(new ChunkId("826", false), "chunkBtn.c2ce9ce9700e2a0ab724.js"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/switch.js"), new ChunkEntry[]
                {
                    new(new ChunkId("chunk1"), "5bc7d56beddd675ee02a.js"),
                    new(new ChunkId("411"), "bee97256afba34de130e.js"),
                    new(new ChunkId("826", false), "c2ce9ce9700e2a0ab724.js"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/ternary.js"), new ChunkEntry[]
                {
                    new(new ChunkId("chunk1"), "5bc7d56beddd675ee02a.js"),
                    new(new ChunkId("411", false), "bee97256afba34de130e.js"),
                    new(new ChunkId("826", false), "c2ce9ce9700e2a0ab724.js"),
                }
            ];
            yield return
            [
                File.ReadAllText("Samples/two-loaders.js"), new ChunkEntry[]
                {
                    new(new ChunkId("400", false), "chunk2.5bc7d56beddd675ee02a.js"),
                    new(new ChunkId("411", false), "chunk1.bee97256afba34de130e.js"),
                    new(new ChunkId("826", false), "chunkBtn.c2ce9ce9700e2a0ab724.js"),
                    new(new ChunkId("400", false), "5bc7d56beddd675ee02a.js"),
                    new(new ChunkId("411", false), "bee97256afba34de130e.js"),
                    new(new ChunkId("826", false), "c2ce9ce9700e2a0ab724.js"),
                }
            ];
        }
    }
}
