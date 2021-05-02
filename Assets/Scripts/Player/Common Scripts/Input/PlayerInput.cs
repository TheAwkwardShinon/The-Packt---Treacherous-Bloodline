// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Player/Common Scripts/Input/PlayerInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInputClass : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputClass()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""4f166136-2563-4f68-9a98-dc8d040a75d5"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""5dedd23b-7384-4188-b629-4cb62f3dc4a0"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""e5857c16-a468-4e3f-a020-5a7ae792bdcf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""b05c04fd-12f2-42fb-8c0d-6eec2ba746e7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DashDirection"",
                    ""type"": ""Value"",
                    ""id"": ""2411180e-e80d-412c-a082-c1264806b0a0"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""4d3bd9c2-8474-4b9e-aad2-15f8c50ae0b8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AttackDirection"",
                    ""type"": ""Value"",
                    ""id"": ""2c042df0-2533-4a3e-9185-2b952010d318"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""transformation"",
                    ""type"": ""Button"",
                    ""id"": ""7f11a834-a531-48ab-8184-1bdc249fce35"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""da7ec799-e9ae-4648-9b35-e4b122c9ac8e"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""e9160236-d4d2-4bdd-893c-445e4118c9d6"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ce97745e-4e9c-4573-9f09-788d7f8c4b95"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""314c0aa4-9a09-49e2-ae93-1c4e77d93775"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""076ab226-1016-4d17-acaf-d2accb45be00"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""041a91a3-716f-4ad3-8751-e1578ece4ae1"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a9f53118-f814-41a2-847a-08fcf396d3c3"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c4199e32-72b9-41a0-86eb-4c9f27d7dd5e"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3e922dcb-acc5-4ba2-99fb-48e34c5a7434"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d298b511-acb0-4abf-9f16-02bef3d654b9"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""33a8201f-6527-41fd-bbb4-3d70c8e29e56"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""DashDirection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""65c8eae2-cf45-49c2-ba91-72102343ee68"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DashDirection"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""260c3993-7db1-4675-b0bf-77ec572c982f"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""DashDirection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""b9da7f35-53f9-4d65-a73f-4f96a381fd7c"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""DashDirection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b238a190-1413-4972-a5cd-d8dc38b9ba3a"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""DashDirection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""bee4374c-b032-4d36-9cf6-11b16e9feb08"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""DashDirection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""e5beb36a-50da-4481-8758-11948429a5d0"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""974d5354-846e-4fcd-ad79-d9ec3767392a"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d8750b61-fc5a-485a-ba7f-32ac0ef194f1"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""84df7939-6657-4658-878d-afdc7880322e"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""AttackDirection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3341afeb-ec1e-4256-b86d-0d1e12795866"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""AttackDirection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c5642f36-779c-4a9e-9416-d36dfca15b89"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""transformation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""595f0228-7fe8-4b8c-bf2c-ee6610893714"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""transformation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UserInterface"",
            ""id"": ""945adec4-4049-4d9f-9c0f-d93db80562af"",
            ""actions"": [
                {
                    ""name"": ""SwitchTabLeft"",
                    ""type"": ""Button"",
                    ""id"": ""ac7e6f17-ae0e-4fba-9593-fd2d52e1d1a8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SwitchTabRight"",
                    ""type"": ""Button"",
                    ""id"": ""ff26be66-8a30-4504-b1b5-de1f52cdc6b3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""NavigateCharactersLeft"",
                    ""type"": ""Button"",
                    ""id"": ""7ef686ef-a4b3-4303-b06a-222751345e08"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""NavigateCharactersRight"",
                    ""type"": ""Button"",
                    ""id"": ""a27d9d2d-afce-4ed8-abd9-3a110888a370"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Deselect"",
                    ""type"": ""Button"",
                    ""id"": ""93ab2182-9ccd-4790-a9e9-a3c8d72d5ed9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""859d710f-c8f2-49c0-9b9a-c8d6afe5719f"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""SwitchTabLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""44c5a91b-a3ef-4fe8-b6bd-9a17ffe083aa"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SwitchTabLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bc97da9e-f6d8-4316-941f-4319f20eb991"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""SwitchTabRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f2169d25-9ac9-421e-a51b-534bcdc34ddc"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SwitchTabRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d330cad9-9435-4b9d-87cd-cb26b50c18c3"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""NavigateCharactersLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fae51978-be00-43b6-9df9-51e7f5f3f0ce"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""NavigateCharactersLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""419d459e-da2d-497a-84c4-536d40536826"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""NavigateCharactersLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7ddcf8cd-1f27-4ab0-9864-03cdc9a2f764"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""NavigateCharactersRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9fd32ed4-eb93-4b02-9113-3a545ee216fa"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""NavigateCharactersRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6d274cdb-8cf9-4339-b7d7-74e65d4fcb62"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""NavigateCharactersRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e132b678-c2e1-40a8-8efd-3c450a721b49"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Deselect"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Movement = m_Gameplay.FindAction("Movement", throwIfNotFound: true);
        m_Gameplay_Jump = m_Gameplay.FindAction("Jump", throwIfNotFound: true);
        m_Gameplay_Dash = m_Gameplay.FindAction("Dash", throwIfNotFound: true);
        m_Gameplay_DashDirection = m_Gameplay.FindAction("DashDirection", throwIfNotFound: true);
        m_Gameplay_Attack = m_Gameplay.FindAction("Attack", throwIfNotFound: true);
        m_Gameplay_AttackDirection = m_Gameplay.FindAction("AttackDirection", throwIfNotFound: true);
        m_Gameplay_transformation = m_Gameplay.FindAction("transformation", throwIfNotFound: true);
        // UserInterface
        m_UserInterface = asset.FindActionMap("UserInterface", throwIfNotFound: true);
        m_UserInterface_SwitchTabLeft = m_UserInterface.FindAction("SwitchTabLeft", throwIfNotFound: true);
        m_UserInterface_SwitchTabRight = m_UserInterface.FindAction("SwitchTabRight", throwIfNotFound: true);
        m_UserInterface_NavigateCharactersLeft = m_UserInterface.FindAction("NavigateCharactersLeft", throwIfNotFound: true);
        m_UserInterface_NavigateCharactersRight = m_UserInterface.FindAction("NavigateCharactersRight", throwIfNotFound: true);
        m_UserInterface_Deselect = m_UserInterface.FindAction("Deselect", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Movement;
    private readonly InputAction m_Gameplay_Jump;
    private readonly InputAction m_Gameplay_Dash;
    private readonly InputAction m_Gameplay_DashDirection;
    private readonly InputAction m_Gameplay_Attack;
    private readonly InputAction m_Gameplay_AttackDirection;
    private readonly InputAction m_Gameplay_transformation;
    public struct GameplayActions
    {
        private @PlayerInputClass m_Wrapper;
        public GameplayActions(@PlayerInputClass wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Gameplay_Movement;
        public InputAction @Jump => m_Wrapper.m_Gameplay_Jump;
        public InputAction @Dash => m_Wrapper.m_Gameplay_Dash;
        public InputAction @DashDirection => m_Wrapper.m_Gameplay_DashDirection;
        public InputAction @Attack => m_Wrapper.m_Gameplay_Attack;
        public InputAction @AttackDirection => m_Wrapper.m_Gameplay_AttackDirection;
        public InputAction @transformation => m_Wrapper.m_Gameplay_transformation;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Dash.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDash;
                @DashDirection.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDashDirection;
                @DashDirection.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDashDirection;
                @DashDirection.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDashDirection;
                @Attack.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                @AttackDirection.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttackDirection;
                @AttackDirection.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttackDirection;
                @AttackDirection.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttackDirection;
                @transformation.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnTransformation;
                @transformation.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnTransformation;
                @transformation.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnTransformation;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
                @DashDirection.started += instance.OnDashDirection;
                @DashDirection.performed += instance.OnDashDirection;
                @DashDirection.canceled += instance.OnDashDirection;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @AttackDirection.started += instance.OnAttackDirection;
                @AttackDirection.performed += instance.OnAttackDirection;
                @AttackDirection.canceled += instance.OnAttackDirection;
                @transformation.started += instance.OnTransformation;
                @transformation.performed += instance.OnTransformation;
                @transformation.canceled += instance.OnTransformation;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);

    // UserInterface
    private readonly InputActionMap m_UserInterface;
    private IUserInterfaceActions m_UserInterfaceActionsCallbackInterface;
    private readonly InputAction m_UserInterface_SwitchTabLeft;
    private readonly InputAction m_UserInterface_SwitchTabRight;
    private readonly InputAction m_UserInterface_NavigateCharactersLeft;
    private readonly InputAction m_UserInterface_NavigateCharactersRight;
    private readonly InputAction m_UserInterface_Deselect;
    public struct UserInterfaceActions
    {
        private @PlayerInputClass m_Wrapper;
        public UserInterfaceActions(@PlayerInputClass wrapper) { m_Wrapper = wrapper; }
        public InputAction @SwitchTabLeft => m_Wrapper.m_UserInterface_SwitchTabLeft;
        public InputAction @SwitchTabRight => m_Wrapper.m_UserInterface_SwitchTabRight;
        public InputAction @NavigateCharactersLeft => m_Wrapper.m_UserInterface_NavigateCharactersLeft;
        public InputAction @NavigateCharactersRight => m_Wrapper.m_UserInterface_NavigateCharactersRight;
        public InputAction @Deselect => m_Wrapper.m_UserInterface_Deselect;
        public InputActionMap Get() { return m_Wrapper.m_UserInterface; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UserInterfaceActions set) { return set.Get(); }
        public void SetCallbacks(IUserInterfaceActions instance)
        {
            if (m_Wrapper.m_UserInterfaceActionsCallbackInterface != null)
            {
                @SwitchTabLeft.started -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnSwitchTabLeft;
                @SwitchTabLeft.performed -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnSwitchTabLeft;
                @SwitchTabLeft.canceled -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnSwitchTabLeft;
                @SwitchTabRight.started -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnSwitchTabRight;
                @SwitchTabRight.performed -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnSwitchTabRight;
                @SwitchTabRight.canceled -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnSwitchTabRight;
                @NavigateCharactersLeft.started -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnNavigateCharactersLeft;
                @NavigateCharactersLeft.performed -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnNavigateCharactersLeft;
                @NavigateCharactersLeft.canceled -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnNavigateCharactersLeft;
                @NavigateCharactersRight.started -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnNavigateCharactersRight;
                @NavigateCharactersRight.performed -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnNavigateCharactersRight;
                @NavigateCharactersRight.canceled -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnNavigateCharactersRight;
                @Deselect.started -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnDeselect;
                @Deselect.performed -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnDeselect;
                @Deselect.canceled -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnDeselect;
            }
            m_Wrapper.m_UserInterfaceActionsCallbackInterface = instance;
            if (instance != null)
            {
                @SwitchTabLeft.started += instance.OnSwitchTabLeft;
                @SwitchTabLeft.performed += instance.OnSwitchTabLeft;
                @SwitchTabLeft.canceled += instance.OnSwitchTabLeft;
                @SwitchTabRight.started += instance.OnSwitchTabRight;
                @SwitchTabRight.performed += instance.OnSwitchTabRight;
                @SwitchTabRight.canceled += instance.OnSwitchTabRight;
                @NavigateCharactersLeft.started += instance.OnNavigateCharactersLeft;
                @NavigateCharactersLeft.performed += instance.OnNavigateCharactersLeft;
                @NavigateCharactersLeft.canceled += instance.OnNavigateCharactersLeft;
                @NavigateCharactersRight.started += instance.OnNavigateCharactersRight;
                @NavigateCharactersRight.performed += instance.OnNavigateCharactersRight;
                @NavigateCharactersRight.canceled += instance.OnNavigateCharactersRight;
                @Deselect.started += instance.OnDeselect;
                @Deselect.performed += instance.OnDeselect;
                @Deselect.canceled += instance.OnDeselect;
            }
        }
    }
    public UserInterfaceActions @UserInterface => new UserInterfaceActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IGameplayActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnDashDirection(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnAttackDirection(InputAction.CallbackContext context);
        void OnTransformation(InputAction.CallbackContext context);
    }
    public interface IUserInterfaceActions
    {
        void OnSwitchTabLeft(InputAction.CallbackContext context);
        void OnSwitchTabRight(InputAction.CallbackContext context);
        void OnNavigateCharactersLeft(InputAction.CallbackContext context);
        void OnNavigateCharactersRight(InputAction.CallbackContext context);
        void OnDeselect(InputAction.CallbackContext context);
    }
}
