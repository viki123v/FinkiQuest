# FinkiQuest

# Za da premestite files prajte vaka:
- Project folderot, kaj so vi sa site raboti, kopirajte go vo dr folder
- Vo kopiraniot project folder izbrisete go project.godot fileot
- Site raboti so gi koristite od fodlerot, sceni , c# files, assets, kopirajte gi vo soodvetniot folder na repovo
- Vekje vo noviov proekt, na site sceni edna po edna ke trebit da kliknite i ke vi izlezit error deka ne mozit da najt tocna pateka, samo kliknejte fix dependencies i posle gore desno samo klikni fix broken.
- Sekade vo kodot kaj so koristite res://, za loading na sceni, zamenete so soodvetna pateka. Go naprev jas so klasa vaka:

```
public class ProjectPath
{
    public static string ScenesPath = "res://FinkiSurvive/FinkiQuest/scenes/";
    public static string DefaultPath = "res://FinkiSurvive/FinkiQuest/";
    public static string MainScenePath = ScenesPath + "level.tscn";
}

```

- Inputs so gi koristite za dvizenje ili za so bilo, nemat da rabotat deka nov proekt e. Odete vo input map vo project settings i klajte si odnovo iminja za niv.Isto i to ke trebit vo kodot da go smenite

### GG
