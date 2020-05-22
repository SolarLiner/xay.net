with import <nixpkgs> {};

stdenv.mkDerivation {
    name = "xay";
    src = ./.;
    nativeBuildInputs = [ dotnet-sdk_3 ];
    buildPhase = ''
        mkdir -p $out
        dotnet publish -c Release -o $out/bin
    '';
}
