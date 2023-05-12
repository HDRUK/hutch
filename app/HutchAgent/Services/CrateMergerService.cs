namespace HutchAgent.Services;

/// <summary>
/// This service merges an output RO-Crate back into its input RO-Crate.
/// </summary>
public class CrateMergerService
{
  /// <summary>
  /// Copy a source zipped RO-Crate into an unzipped destination RO-Crate and zip the
  /// destination RO-Crate.
  /// </summary>
  /// <param name="sourceZip"></param>
  /// <param name="mergeInto"></param>
  /// <exception cref="DirectoryNotFoundException">
  /// The directory you are attempting to merge doesn't exists.
  /// The parent of the destination directory couldn't be found or does not exists.
  /// </exception>
  public void MergeCrates(string sourceZip, string mergeInto)
  {
    // Get information on destination and make sure it exists
    var destinationInfo = new DirectoryInfo(mergeInto);
    if (!destinationInfo.Exists) throw new DirectoryNotFoundException($"{mergeInto} does not exist.");

    // Get the parent directory of the merge destination
    var parentInfo = destinationInfo.Parent;
    if (parentInfo is null) throw new DirectoryNotFoundException($"Cannot get parent of {mergeInto}.");
    var parent = parentInfo!.ToString();

    // Copy the result RO-Crate (`file`) into the unzipped original RO-Crate
    File.Copy(sourceZip, Path.Combine(mergeInto, Path.GetFileName(sourceZip)));

    // Todo: Alter the input RO-Crate ro-crate-metadata.json

    // Zip the original crate, now containing the zipped result crate, and upload to S3.
    // var fileNoExt = destinationInfo.Name;
    // var zipFile = $"{fileNoExt}-merged.zip";
    // ZipFile.CreateFromDirectory(mergeInto, Path.Combine(parent, zipFile));
  }
}
