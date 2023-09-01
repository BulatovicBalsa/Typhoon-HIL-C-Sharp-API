# --------------------------------------------------------------
# This example demonstrates how to use Configuration Manager
# API to load a project, create configurations programmatically,
# load saved configurations and generate the .tse models.
# --------------------------------------------------------------

from typhoon.api.configuration_manager import ConfigurationManagerAPI
import os

# create a Configuration Manager API instance
cm = ConfigurationManagerAPI()

# Define files/folders
prj_file = "example_project.cmp" # project file
cfg_folder = "./configs" # folder with configuration files
out_folder = "./output" # output folder

# load project
prj = cm.load_project(prj_file)

# list of configurations to generate
cfgs = []

# create configuration programmatically and add it to the list (x4)
cfgs.append(cm.create_config("PFE_IM_LP"))
cm.picks(
    cfgs[-1],
    [
    cm.make_pick("Rectifier", "Diode rectifier"),
    cm.make_pick("Motor", "Induction low power")
    ]
)

cfgs.append(cm.create_config("AFE_IM_LP"))
cm.picks(
    cfgs[-1],
    [
    cm.make_pick("Rectifier", "Thyristor rectifier"),
    cm.make_pick("Motor", "Induction low power")
    ]
)

cfgs.append(cm.create_config("PFE_PMSM_LP"))
cm.picks(
    cfgs[-1],
    [
    cm.make_pick("Rectifier", "Diode rectifier"),
    cm.make_pick("Motor", "PMSM low power")
    ]
)

cfgs.append(cm.create_config("AFE_PMSM_LP"))
cm.picks(
    cfgs[-1],
    [
    cm.make_pick("Rectifier", "Thyristor rectifier"),
    cm.make_pick("Motor", "PMSM low power")
    ]
)

# Load configurations from files
# get the list of configuration files
cfg_file_list = os.listdir(cfg_folder)

# load configurations from files and add them to the configuration list
for cfg_file in cfg_file_list:
    cfgs.append(cm.load_config(cfg_folder + "/" + cfg_file))

# Generate models
print("Generating models:")
for ind, cfg in enumerate(cfgs):
    cfg_name = cm.get_name(cfg)
    print(str(ind+1) + " / " + str(len(cfgs)) + " : " + cfg_name)
    # tse file name can be overriden by specifying the file_name parameter
    cm.generate(prj, cfg, out_dir = out_folder)

print("Models are stored in the " + out_folder + " folder.")

