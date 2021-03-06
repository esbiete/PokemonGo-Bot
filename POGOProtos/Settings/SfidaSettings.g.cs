// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: POGOProtos/Settings/SfidaSettings.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace POGOProtos.Settings {

  /// <summary>Holder for reflection information generated from POGOProtos/Settings/SfidaSettings.proto</summary>
  public static partial class SfidaSettingsReflection {

    #region Descriptor
    /// <summary>File descriptor for POGOProtos/Settings/SfidaSettings.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static SfidaSettingsReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CidQT0dPUHJvdG9zL1NldHRpbmdzL1NmaWRhU2V0dGluZ3MucHJvdG8SE1BP",
            "R09Qcm90b3MuU2V0dGluZ3MiLgoNU2ZpZGFTZXR0aW5ncxIdChVsb3dfYmF0",
            "dGVyeV90aHJlc2hvbGQYASABKAJiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::POGOProtos.Settings.SfidaSettings), global::POGOProtos.Settings.SfidaSettings.Parser, new[]{ "LowBatteryThreshold" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class SfidaSettings : pb::IMessage<SfidaSettings> {
    private static readonly pb::MessageParser<SfidaSettings> _parser = new pb::MessageParser<SfidaSettings>(() => new SfidaSettings());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SfidaSettings> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::POGOProtos.Settings.SfidaSettingsReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SfidaSettings() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SfidaSettings(SfidaSettings other) : this() {
      lowBatteryThreshold_ = other.lowBatteryThreshold_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SfidaSettings Clone() {
      return new SfidaSettings(this);
    }

    /// <summary>Field number for the "low_battery_threshold" field.</summary>
    public const int LowBatteryThresholdFieldNumber = 1;
    private float lowBatteryThreshold_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public float LowBatteryThreshold {
      get { return lowBatteryThreshold_; }
      set {
        lowBatteryThreshold_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as SfidaSettings);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(SfidaSettings other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (LowBatteryThreshold != other.LowBatteryThreshold) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (LowBatteryThreshold != 0F) hash ^= LowBatteryThreshold.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (LowBatteryThreshold != 0F) {
        output.WriteRawTag(13);
        output.WriteFloat(LowBatteryThreshold);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (LowBatteryThreshold != 0F) {
        size += 1 + 4;
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(SfidaSettings other) {
      if (other == null) {
        return;
      }
      if (other.LowBatteryThreshold != 0F) {
        LowBatteryThreshold = other.LowBatteryThreshold;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 13: {
            LowBatteryThreshold = input.ReadFloat();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
